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
    private Transform boardHolder;
    private Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2>();

    public void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector2 (x, y), new Vector2 (x, y));

                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0), Quaternion.identity);
                
                instance.transform.SetParent(boardHolder, true);
            }
        }
    }
}
