using Codice.CM.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TileType
{
    ESSENTIAL,
    RANDOM,
    EMPTY,
    CHEST,
    ENEMY,
}

public enum UnitType
{
    ENEMY,
    WALL,
    CHEST,
    EXIT,
}

public class DungeonManager : MonoBehaviour
{
    //접근 불가능한 유닛이 존재하는 경우 추가, 없어진경우 삭제
    public static Dictionary<Vector2, UnitType> unitPositions = new Dictionary<Vector2, UnitType>();

    [Serializable]
    public class PathTile
    {
        public TileType type;
        public Vector2 position;
        public List<Vector2> adjacentPathTiles;

        public PathTile (TileType _t, Vector2 _p, int _min, int _max, Dictionary<Vector2, TileType> _currentTiles)
        {
            type = _t;
            position = _p;
            adjacentPathTiles = getAdjacentPath(_min, _max, _currentTiles);
        }

        /// <summary>
        /// 인접한 (상하좌우) 타일을 반환
        /// </summary>
        public List<Vector2> getAdjacentPath(int _minBound, int _maxBound, Dictionary<Vector2, TileType> _currentTiles)
        {
            List<Vector2> pathTiles = new List<Vector2>();
            
            if (position.y + 1 < _maxBound && !_currentTiles.ContainsKey(new Vector2(position.x, position.y + 1)))
            {
                pathTiles.Add(new Vector2(position.x, position.y + 1));
            }

            if (position.x + 1 < _maxBound && !_currentTiles.ContainsKey(new Vector2(position.x + 1, position.y)))
            {
                pathTiles.Add(new Vector2(position.x + 1, position.y));
            }

            if (position.y - 1 >= _minBound && !_currentTiles.ContainsKey(new Vector2(position.x, position.y - 1)))
            {
                pathTiles.Add(new Vector2(position.x, position.y - 1));
            }

            if (position.x - 1 >= _minBound && !_currentTiles.ContainsKey(new Vector2(position.x - 1, position.y)) && type != TileType.ESSENTIAL)
            {
                pathTiles.Add(new Vector2(position.x - 1, position.y));
            }

            return pathTiles;
        }
    }

    public Dictionary<Vector2, TileType> gridPositions = new Dictionary<Vector2, TileType>();
    public int minBound = 0;
    public int maxBound;
    public static Vector2 startPos;
    public Vector2 endPos;

    public void StartDungeon()
    {
        Random.InitState(GameManager.seed);
        gridPositions.Clear();

        maxBound = Random.Range(50, 101);

        BuildEssentialPath();
        BuildRandomPath();
    }

    /// <summary>
    /// 필수경로
    /// </summary>
    private void BuildEssentialPath()
    {
        int randomY = Random.Range(0, maxBound + 1);
        PathTile ePath = new PathTile(TileType.ESSENTIAL, new Vector2(0, randomY), minBound, maxBound, gridPositions);
        startPos = ePath.position;
        Player.Instance.SetPosition(startPos);

        int boundTracker = 0;

        //maxbound가 되거나 남은 탐색 타일 카운트가 0이 되면 종료
        while (boundTracker < maxBound)
        {
            gridPositions.Add(ePath.position, TileType.EMPTY);
            int adjacentTileCount = ePath.adjacentPathTiles.Count;
            int randomIndex = Random.Range(0, adjacentTileCount);
            Vector2 nextPathPos;

            if (adjacentTileCount > 0)
                nextPathPos = ePath.adjacentPathTiles[randomIndex];
            else
                break;

            PathTile nextPath = new PathTile(TileType.ESSENTIAL, nextPathPos, minBound, maxBound, gridPositions);

            if (nextPath.position.x > ePath.position.x || Random.Range(0, 2) == 1)
            {
                ++boundTracker;
            }
            ePath = nextPath;
        }

        if (!gridPositions.ContainsKey(ePath.position))
            gridPositions.Add(ePath.position, TileType.EMPTY);

        endPos = new Vector2(ePath.position.x, ePath.position.y);
    }

    /// <summary>
    /// 파생경로
    /// </summary>
    private void BuildRandomPath()
    {
        List<PathTile> pathQueue = new List<PathTile>();

        foreach (KeyValuePair<Vector2, TileType> tile in gridPositions)
        {
            Vector2 tilePos = new Vector2(tile.Key.x, tile.Key.y);
            pathQueue.Add(new PathTile(TileType.RANDOM, tilePos, minBound, maxBound, gridPositions));
        }

        for (int i = 0; i < pathQueue.Count; ++i)
        {
            PathTile _tile = pathQueue[i];
            int adjacentTileCount = _tile.adjacentPathTiles.Count;

            if (adjacentTileCount == 0) return;

            if (Random.Range(0, 5) == 1)
            {
                BuildRandomChamber(_tile);
            }
            else if (Random.Range(0, 5) == 1 || (_tile.type == TileType.RANDOM && adjacentTileCount > 1))
            {
                int randomIndex = Random.Range(0, adjacentTileCount);
                Vector2 newRPathPos = _tile.adjacentPathTiles[randomIndex];

                if (gridPositions.ContainsKey(newRPathPos)) continue;
                
                if (Random.Range(0, GameManager.Instance.enemySpawnRatio) == 1)
                {
                    gridPositions.Add(newRPathPos, TileType.ENEMY);
                }
                else
                {
                    gridPositions.Add(newRPathPos, TileType.EMPTY);
                }

                PathTile newRPath = new PathTile(TileType.RANDOM, newRPathPos, minBound, maxBound, gridPositions);
                pathQueue.Add(newRPath);
            }
        }
    }

    private void BuildRandomChamber(PathTile _tile)
    {
        int chamberSize = 3;
        int adjacentTileCount = _tile.adjacentPathTiles.Count;
        int randomIndex = Random.Range(0, adjacentTileCount);
        Vector2 chamberOrigin = _tile.adjacentPathTiles[randomIndex];

        for(int x = (int)chamberOrigin.x; x < chamberOrigin.x + chamberSize; x++)
        {
            for (int y = (int)chamberOrigin.y; y < chamberOrigin.y + chamberSize; y++)
            {
                Vector2 chamberTilePos = new Vector2(x, y);

                bool IsTileInBound = chamberTilePos.x < maxBound && chamberTilePos.x > 0 && chamberTilePos.y < maxBound && chamberTilePos.y > 0;
                if (!gridPositions.ContainsKey(chamberTilePos) && IsTileInBound)
                {
                    if (Random.Range(0, 70) == 1)
                    {
                        gridPositions.Add(chamberTilePos, TileType.CHEST);
                    }
                    else
                        gridPositions.Add(chamberTilePos, TileType.EMPTY);
                }
            }
        }
    }
}
