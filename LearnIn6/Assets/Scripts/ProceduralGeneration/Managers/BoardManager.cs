using System.Collections.Generic;
using UnityEngine;

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
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public Transform boardHolder;
    private Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2>();

    public void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (gridPositions.ContainsKey(new Vector2(x, y))) continue;

                gridPositions.Add(new Vector2 (x, y), new Vector2 (x, y));

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
            
            if (Random.Range(0, 3) == 1)
            {
                toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
            }

            GameObject instance = Instantiate(toInstantiate, (Vector3)_pos, Quaternion.identity);
            instance.transform.SetParent(boardHolder);
        }

        
    }
}
