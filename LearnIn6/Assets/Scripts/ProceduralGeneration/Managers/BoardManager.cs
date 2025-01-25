using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    public class Count
    {
        public int min;
        public int max;
        public Count(int _min, int _max)
        {
            min = _min;
            max = _max;
        }
    }

    public int columns = 5;
    public int rows = 5;
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] outerWallTiles;
    public ItemType[] itemTypes; 
    public GameObject chestTile;
    public GameObject enemy;

    [HideInInspector] public Transform boardHolder;

    private Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2>();
    private Transform dungeonBoardHolder;

    public void BoardSetup()
    {
        Random.InitState(GameManager.seed);

        if (boardHolder == null) boardHolder = new GameObject("Board").transform;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (gridPositions.ContainsKey(new Vector2(x, y))) continue;

                gridPositions.Add(new Vector2(x, y), new Vector2(x, y));

                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0), Quaternion.identity);

                instance.transform.SetParent(boardHolder, true);
            }
        }
    }

    /// <param name="_hor">only -1, 0, 1</param>
    /// <param name="_ver">only -1, 0, 1</param>
    public void addToBoard(int _hor, int _ver)
    {
        if (_hor != 0)
        {
            //시야 내에 타일이 존재하는지 확인
            int x = (int)Player.position.x;
            int sightX = x + (_hor * 2);

            for (x += _hor; _hor > 0 ? x <= sightX : x >= sightX; x = x + _hor)
            {
                int y = (int)Player.position.y;
                int sightY = y + 1;

                for (y -= 1; y <= sightY; y++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }
        else if (_ver != 0)
        {
            int y = (int)Player.position.y;
            int sightY = y + (_ver * 2);

            for (y += _ver; _ver > 0 ? y <= sightY : y >= sightY; y = y + _ver)
            {
                int x = (int)Player.position.x;
                int sightX = x + 1;

                for (x -= 1; x <= sightX; x++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }
    }

    private void addTiles(Vector2 _pos)
    {
        if (!gridPositions.ContainsKey(_pos))
        {
            gridPositions.Add(_pos, _pos);
            GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
            GameObject instance = Instantiate(toInstantiate, (Vector3)_pos, Quaternion.identity);
            instance.transform.SetParent(boardHolder);

            toInstantiate = null;

            if (Random.Range(0, 3) == 1)
            {
                toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
            }
            else if (Random.Range(0, 100) == 1)
            {
                toInstantiate = exit;
            }
            
            if (Random.Range(0, GameManager.Instance.enemySpawnRatio) == 1)
            {
                toInstantiate = enemy;
            }

            if (toInstantiate == null) return;

            instance = Instantiate(toInstantiate, (Vector3)_pos, Quaternion.identity);
            instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
            instance.transform.SetParent(boardHolder);
        }
    }

    public void SetDungeonBoard(Dictionary<Vector2,TileType> _dungeonTiles, int _bound, Vector2 _endPos)
    {
        if (boardHolder == null) boardHolder = new GameObject("Board").transform;
        boardHolder.gameObject.SetActive(false);
        dungeonBoardHolder = new GameObject("Dungeon").transform;
        GameObject toInstantiate, instance;

        foreach(KeyValuePair<Vector2, TileType> tile in _dungeonTiles)
        {
            toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
            instance = Instantiate(toInstantiate, (Vector3)tile.Key, Quaternion.identity);
            instance.transform.SetParent(dungeonBoardHolder);

            if (tile.Value == TileType.CHEST)
            {
                toInstantiate = chestTile;
                instance = Instantiate(toInstantiate, (Vector3)tile.Key, Quaternion.identity);
                instance.transform.SetParent(dungeonBoardHolder);
                DungeonManager.unitPositions.Add(tile.Key, UnitType.CHEST);
            }
            else if (tile.Value == TileType.ENEMY)
            {
                toInstantiate = enemy;
                instance = Instantiate(toInstantiate, (Vector3)tile.Key, Quaternion.identity);
                instance.transform.SetParent(dungeonBoardHolder);
                DungeonManager.unitPositions.Add(tile.Key, UnitType.ENEMY);
            }
        }

        for(int x = -1; x < _bound + 1; x++)
        {
            for(int y = -1; y < _bound; y++)
            {
                Vector2 pos = new Vector2(x, y);
                if (!_dungeonTiles.ContainsKey(pos))
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    instance = Instantiate(toInstantiate, pos, Quaternion.identity);
                    instance.transform.SetParent(dungeonBoardHolder);
                    DungeonManager.unitPositions.Add(pos, UnitType.WALL);
                }
            }
        }

        toInstantiate = exit;
        instance = Instantiate(toInstantiate, (Vector3)_endPos, Quaternion.identity);
        instance.transform.SetParent(dungeonBoardHolder);
        DungeonManager.unitPositions.Add(_endPos, UnitType.EXIT);
    }

    public void SetWorldBoard()
    {
        Destroy(dungeonBoardHolder.gameObject);
        boardHolder.gameObject.SetActive(true);
    }

    public bool CheckValidTile(Vector2 _pos)
    {
        return gridPositions.ContainsKey(_pos);
    }
}
