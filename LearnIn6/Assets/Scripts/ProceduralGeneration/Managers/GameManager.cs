using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float turnDelay = 0.1f;
    public int healthPoints = 100;

    private BoardManager boardManager;
    private List<Enemy> enemies;
    private bool enemiesMoving;

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
        boardManager = GetComponent<BoardManager>();
        InitGame();
    }
    
    private void InitGame()
    {
        enemies.Clear();
        boardManager.BoardSetup();
    }
}
