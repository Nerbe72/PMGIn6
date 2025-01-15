using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject
{
    public static Player Instance;
    public static Vector2 position;
    public static bool isFacingRight = true;

    public bool onWorldBoard;
    public bool dungeonTransition;

    public int wallDamage = 1;
    public TMP_Text healthText;
    public Gradient healthGraident;

    public Image glove;
    public Image boot;

    public int attackMod = 0;
    public int defenseMod = 0;

    public Image weaponComponent1, weaponComponent2, weaponComponent3;

    private Animator animator;
    private int health;
    private Dictionary<string, Item> inventory;
    private Weapon weapon;

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
        healthText.color = healthGraident.Evaluate(health/100f);
        onWorldBoard = true;
        dungeonTransition = false;

        inventory = new Dictionary<string, Item>();
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

        if (horizontal == 0 && vertical == 0) return;
        if (dungeonTransition) return;

        canMove = AttemptMove<Wall>(horizontal, vertical);
        canMove = AttemptMove<Chest>(horizontal, vertical);

        if (canMove && onWorldBoard)
        {
            position.x += horizontal;
            position.y += vertical;
            GameManager.Instance.updateBoard(horizontal,vertical);
        }
    }

    public void SetPosition(Vector2 _pos)
    {
        transform.position = _pos;
        position = _pos;
    }

    protected override bool AttemptMove<T>(int _xDir, int _yDir)
    {
        if (_xDir > 0 && !isFacingRight)
        {
            isFacingRight = true;
        } else if (_xDir < 0 && isFacingRight)
        {
            isFacingRight = false;
        }

        if (_xDir > 0)
        {
            transform.eulerAngles = Vector3.zero;
        } else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        bool hit = base.AttemptMove<T>((int)position.x + _xDir,(int)position.y + _yDir);

        GameManager.Instance.playersTurn = false;

        return hit;
    }

    protected override void OnCantMove<T>(T component)
    {
        if (typeof(T) == typeof(Wall))
        {
            (component as Wall).DamageWall(wallDamage);
        }

        if (typeof(T) == typeof(Chest))
        {
            (component as Chest).Open();
        }

        if (weapon != null)
        {
            weapon.UseWeapon();
        }

        animator.SetTrigger("Chop");
    }

    private void GoDungeonPortal()
    {
        if (onWorldBoard)
        {
            onWorldBoard = false;
            GameManager.Instance.enterDungeon();
            transform.position = DungeonManager.startPos;
        }
        else
        {
            onWorldBoard = true;
            GameManager.Instance.exitDungeon();
            transform.position = position;
        }
    }

    private void UpdateHealth(Collider2D _item)
    {
        if (health >= 100) return;

        if (_item.tag == "Food")
        {
            health += Random.Range(1, 4);
        }
        else
        {
            health += Random.Range(1, 11);
        }

        GameManager.Instance.healthPoints = health;
        healthText.text = "Health: " + health;
        healthText.color = healthGraident.Evaluate(health/100f);
    }

    private void UpdateInventory(Collider2D _item)
    {
        Item itemData = _item.GetComponent<Item>();

        if (!inventory.ContainsKey(itemData.currentType.name))
            inventory.Add(itemData.currentType.name, itemData);
        else
            inventory[itemData.currentType.name] = itemData;

        //인벤토리 이미지 수정
        if (itemData.name == "Glove")
            glove.color = itemData.level;
        else
            boot.color = itemData.level;

        attackMod = 0;
        defenseMod = 0;

        foreach(KeyValuePair<string, Item> gear in inventory)
        {
            attackMod += gear.Value.attackMod;
            defenseMod += gear.Value.defenseMod;
        }

        if (weapon != null)
        {
            wallDamage = attackMod + 3;
        }
    }

    private void OnTriggerEnter2D(Collider2D _collider)
    {
        if (_collider == null) return;

        if (_collider.tag == "Exit")
        {
            dungeonTransition = true;
            Invoke("GoDungeonPortal", 0.5f);
            Destroy(_collider.gameObject);
        }
        else if (_collider.tag == "Food" || _collider.tag == "Soda")
        {
            UpdateHealth(_collider);
            Destroy(_collider.gameObject);
        }
        else if (_collider.tag == "Item")
        {
            UpdateInventory(_collider);
            Destroy(_collider.gameObject);
        }
        else if (_collider.tag == "Weapon")
        {
            if (weapon != null)
            {
                Destroy(weapon.gameObject);
            }
            _collider.enabled = false;
            _collider.transform.parent = transform;
            weapon = _collider.GetComponent<Weapon>();
            weapon.AquireWeapon();
            weapon.isPlayerInventory = true;
            weapon.EnableSpriteRenderer(false);
            wallDamage = attackMod + 3;
            weaponComponent1.sprite = weapon.GetComponentImage(0);
            weaponComponent2.sprite = weapon.GetComponentImage(1);
            weaponComponent3.sprite = weapon.GetComponentImage(2);
            weaponComponent1.color = Color.white;
            weaponComponent2.color = Color.white;
            weaponComponent3.color = Color.white;
        }
    }
}
