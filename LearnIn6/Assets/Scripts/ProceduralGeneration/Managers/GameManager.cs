using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float turnDelay = 0.1f;
    public int healthPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private BoardManager boardManager;
    private List<Enemy> enemies;
    private bool enemiesMoving = false;

    private Dictionary<Vector2, Vector2> gridPositions;

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
        InitGame();
        SceneManager.sceneLoaded += WhenSceneLoaded;
    }

    private void Update()
    {
        if (playersTurn || enemiesMoving) return;

        StartCoroutine(MoveEnemies());
    }

    private void WhenSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        InitGame();
    }

    private void InitGame()
    {
        enemies.Clear();
        boardManager.BoardSetup();
    }

    public void GameOver()
    {
        enabled = false;
    }

    public void updateBoard(int _hor, int _ver)
    {
        boardManager.addToBoard(_hor, _ver);
    }

    private IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        playersTurn = true;
        enemiesMoving = false;
        yield break;
    }
}
