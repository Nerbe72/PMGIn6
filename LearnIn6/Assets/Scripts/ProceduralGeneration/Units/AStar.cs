using NUnit.Framework;
using System.Collections.Generic;
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

        public Node(Vector2 _pos)
        {
            position = _pos;
        }
    }

    public List<Node> FindPath(Vector2 _start, Vector2 _end)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        Node startNode = new Node(_start);
        Node endNode = new Node(_end);

        openList.Add(startNode);

        while(openList.Count > 0)
        {
            Node currentNode = openList[0];
            foreach(Node node in openList)
            {
                if (node.fCost < currentNode.fCost || (node.fCost == currentNode.fCost && node.hCost < currentNode.hCost))
                {
                    currentNode = node;
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.position == endNode.position)
            {
                return FinalPath(startNode, endNode);
            }

            foreach(Node neighbor in GetNeighbor(currentNode))
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                float newGCost = currentNode.gCost + Vector2.Distance(currentNode.position, neighbor.position);
                if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
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

        return null;
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
        return new List<Node>();
    }
}
