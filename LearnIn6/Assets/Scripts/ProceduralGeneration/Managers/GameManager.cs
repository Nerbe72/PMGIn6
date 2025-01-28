using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int seed;

    public float turnDelay = 0.1f;
    public int healthPoints = 100;
    public bool playersTurn = true;

    public bool enemiesFaster = false;
    public bool enemiesSmarter = false;
    public bool enemiesSmartest = false;
    public int enemySpawnRatio = 20;

    private BoardManager boardManager;
    public DungeonManager dungeonManager;
    private Player player;
    private List<Enemy> enemies;
    private bool enemiesMoving = false;
    [SerializeField] private bool playerInDungeon;

    private Dictionary<Vector2, Vector2> gridPositions;

    Coroutine enemyCo;

    private void Awake()
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

        enemies = new List<Enemy>();
        gridPositions = new Dictionary<Vector2, Vector2>();
        boardManager = GetComponent<BoardManager>();
        dungeonManager = GetComponent<DungeonManager>();

        seed = (int)System.DateTime.Now.Ticks;
        //seed = -714783279;
    }

    private void Start()
    {
        player = Player.Instance;
        SceneManager.sceneLoaded += WhenSceneLoaded;
        InitGame();
    }

    private void Update()
    {
        if (playersTurn || enemiesMoving) return;

        if (enemyCo != null)
        {
            StopCoroutine(enemyCo);
        }
        enemyCo = StartCoroutine(MoveEnemies());
    }

    private void WhenSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        InitGame();
    }

    private void InitGame()
    {
        playerInDungeon = false;
        //boardManager.BoardSetup();
        enterDungeon();
    }

    public void GameOver()
    {
        enabled = false;
    }

    public void updateBoard(int _hor, int _ver)
    {
        boardManager.addToBoard(_hor, _ver);
    }

    public void enterDungeon()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i].gameObject);
        }
        enemies.Clear();

        dungeonManager.StartDungeon();
        boardManager.SetDungeonBoard(dungeonManager.gridPositions, dungeonManager.maxBound, dungeonManager.endPos);
        player.dungeonTransition = false;

        playerInDungeon = true;
    }

    public void exitDungeon()
    {
        boardManager.SetWorldBoard();
        player.dungeonTransition = false;
        playerInDungeon = false;
        enemies.Clear();
        SoundManager.Instance.FormAudio(false);
    }

    public void AddEnemyToList(Enemy _enemy)
    {
        enemies.Add(_enemy);
        SoundManager.Instance.FormAudio(true);
    }

    public void RemoveEnemyFromList(Enemy _enemy)
    {
        if (enemies.Contains(_enemy))
            enemies.Remove(_enemy);

        if (enemies.Count == 0)
        {
            SoundManager.Instance.FormAudio(false);
        }
    }

    public bool CheckValidTile(Vector2 _pos)
    {
        return gridPositions.ContainsKey(_pos);
    }

    public void RemoveEnemy(Enemy _target)
    {
        enemies.Remove(_target);
    }

    private IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        List<Enemy> enemiesToDestroy = new List<Enemy>();
        for (int i = 0; i < enemies.Count; i++)
        {
            if (playerInDungeon)
            {
                if (!enemies[i].GetSpriteRenderer().isVisible)
                {
                    if (i == enemies.Count - 1)
                    {
                        yield return new WaitForSeconds(enemies[i].moveTime);
                    }
                    continue;
                }
            }
            else
            {
                if ((!enemies[i].GetSpriteRenderer().isVisible) || (!boardManager.CheckValidTile(enemies[i].transform.position)))
                {
                    enemiesToDestroy.Add(enemies[i]);
                    continue;
                }
            }

            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;

        for (int i = 0; i < enemiesToDestroy.Count; i++)
        {
            if (enemies.Contains(enemiesToDestroy[i]))
                enemies.Remove(enemiesToDestroy[i]);
            Destroy(enemiesToDestroy[i].gameObject);
        }
        enemiesToDestroy.Clear();

        enemyCo = null;
        yield break;
    }
}
