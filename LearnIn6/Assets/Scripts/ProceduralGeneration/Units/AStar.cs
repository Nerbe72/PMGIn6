using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public class Node
    {
        public Vector2 position;
        public float gCost;
        public float hCost;
        public float fCost => gCost + hCost;
        public Node parent;
        public bool isMovable;

        public Node(Vector2 _pos, bool _isMovable)
        {
            position = _pos;
            isMovable = _isMovable;
        }

        public override bool Equals(object obj)
        {
            if (obj is Node other)
            {
                return this.position == other.position;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }

    private List<Node> bestPath = new List<Node>();
    private int searchFailCount = 0;

    private Coroutine findCo;

    public void FindPath(Vector2 _start, Vector2 _end)
    {
        bestPath.Clear();

        HashSet<Node> openList = new HashSet<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        Node startNode = new Node(_start, true);
        Node endNode = new Node(_end, true);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            if (openList.Count >= 200) break;

            Node currentNode = openList.ElementAt(0);

            foreach (Node node in openList)
            {
                if (node.fCost < currentNode.fCost || (node.fCost == currentNode.fCost && node.hCost < currentNode.hCost))
                {
                    currentNode = node;
                }
            }

            //탐색 완료 플래그
            openList.Remove(currentNode);
            if (!closedList.Contains(currentNode))
                closedList.Add(currentNode);

            if (currentNode.position == endNode.position)
            {
                //return FinalPath(startNode, endNode);
                bestPath = FinalPath(startNode, currentNode);
                break;
            }

            foreach (Node neighbor in GetNeighbor(currentNode))
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                if (!neighbor.isMovable)
                {
                    closedList.Add(neighbor);
                    continue;
                }

                float newGCost = currentNode.gCost + 1;
                if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = Mathf.Abs(neighbor.position.x - endNode.position.x) + Mathf.Abs(neighbor.position.y - endNode.position.y);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 다음 이동할 노드를 반환
    /// </summary>
    /// <returns></returns>
    public Node GetNextNode()
    {
        if (bestPath == null || bestPath.Count <= 1)
            return new Node(transform.position, true);

        return bestPath[1];
    }

    public List<Vector3> GetVecPath()
    {
        List<Vector3> vPath = new List<Vector3>();

        foreach(Node p in bestPath)
        {
            vPath.Add((Vector3)p.position);
        }

        return vPath;
    }

    /// <summary>
    /// end노드부터 start노드까지 부모노드를 순회하며 경로를 생성함
    /// </summary>
    /// <param name="_startNode"></param>
    /// <param name="_endNode"></param>
    /// <returns></returns>
    private List<Node> FinalPath(Node _startNode, Node _endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = _endNode;

        while(currentNode != _startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private List<Node> GetNeighbor(Node _targetNode)
    {
        List<Node> neighbors = new List<Node>();
        Vector2[] directions = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1),
        };

        Vector2 neighborPos;

        foreach(Vector2 dir in directions)
        {
            neighborPos = _targetNode.position + dir;
            neighbors.Add(new Node(neighborPos, !DungeonManager.unitPositions.ContainsKey(neighborPos)));
        }

        return neighbors;
    }
}
