using System;
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
            gCost = float.MaxValue; // �ʱ� gCost�� �ſ� ũ�� ����
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

        startNode.gCost = 0;
        startNode.hCost = Vector2.Distance(startNode.position, endNode.position);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            //f�� �����鼭 h�� ���� ������
            Node currentNode = openList.OrderBy(n => n.fCost).ThenBy(n => n.hCost).First();

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.position == endNode.position)
            {
                bestPath = FinalPath(startNode, currentNode);
                break;
            }

            foreach (Node neighbor in GetNeighbor(currentNode))
            {
                if (closedList.Contains(neighbor))
                    continue;

                if (!neighbor.isMovable)
                {
                    closedList.Add(neighbor);
                    continue;
                }

                //g/h�ڽ�Ʈ �翬��
                float newGCost = currentNode.gCost + 1;
                if (newGCost < neighbor.gCost)
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = Vector2.Distance(neighbor.position, endNode.position);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        GC.Collect();
    }

    /// <summary>
    /// ���� �̵��� ��带 ��ȯ
    /// </summary>
    public Node GetNextNode()
    {
        //��ΰ� ������ ���ڸ� ���
        if (bestPath == null || bestPath.Count <= 1)
            return new Node((Vector2)transform.position, true);

        return bestPath[0];
    }

    public List<Vector3> GetVecPath()
    {
        List<Vector3> vPath = new List<Vector3>();

        foreach (Node p in bestPath)
        {
            vPath.Add((Vector3)p.position);
        }

        return vPath;
    }

    /// <summary>
    /// End ������ Start ������ �θ� ��带 ��ȸ�ϸ� ��θ� ������
    /// </summary>
    private List<Node> FinalPath(Node _startNode, Node _endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = _endNode;

        while (currentNode != _startNode)
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

        foreach (Vector2 dir in directions)
        {
            Vector2 neighborPos = _targetNode.position + dir;
            bool isMovable = !DungeonManager.unitPositions.ContainsKey(neighborPos);

            neighbors.Add(new Node(neighborPos, isMovable));
        }

        return neighbors;
    }
}
