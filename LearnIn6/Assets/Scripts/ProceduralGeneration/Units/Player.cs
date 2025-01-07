using TMPro;
using UnityEngine;

public class Player : MovingObject
{
    public static Player Instance;
    public static Vector2 position;

    public int wallDamage = 1;
    public TMP_Text healthText;

    private Animator animator;
    private int health;

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        animator = GetComponent<Animator>();
        base.Awake();
    }

    protected void Start()
    {
        health = GameManager.Instance.healthPoints;
        healthText.text = "Health " + health;
        transform.position = new Vector2(2, 2);
        position = new Vector2(2, 2);
    }

    void Update()
    {
        if (!GameManager.Instance.playersTurn) return;

        int horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        int vertical = (int)(Input.GetAxisRaw("Vertical"));

        bool canMove = false;

        //좌우이동 선입력
        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
        {
            Debug.Log(position);
            canMove = AttemptMove<Wall>((int)position.x + horizontal, (int)position.y + vertical);

            if (canMove)
            {
                position.x += horizontal;
                position.y += vertical;
                GameManager.Instance.updateBoard(horizontal, vertical);
            }
            Debug.Log(position);
        }
    }

    protected override bool AttemptMove<T>(int _xDir, int _yDir)
    {
        bool hit = base.AttemptMove<T>(_xDir, _yDir);

        GameManager.Instance.playersTurn = false;

        return hit;
    }

    protected override void OnCantMove<T>(T component)
    {
        
    }
}
