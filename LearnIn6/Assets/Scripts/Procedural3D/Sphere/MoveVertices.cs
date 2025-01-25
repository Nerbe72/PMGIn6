using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

public class MoveVertices : MonoBehaviour
{
    MeshFilter meshFilter;
    Vector3[] vertices;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
        meshFilter.mesh.vertices = Randomize(vertices);
    }

    Vector3[] Randomize(Vector3[] _verts)
    {
        Dictionary<Vector3, List<int>> dictionary = new Dictionary<Vector3, List<int>>();

        for(int x = 0; x < _verts.Length; x++)
        {
            if (!dictionary.ContainsKey(_verts[x]))
            {
                dictionary.Add(_verts[x], new List<int>());
            }
            dictionary[_verts[x]].Add(x);
        }

        foreach (KeyValuePair<Vector3, List<int>> pair in dictionary)
        {
            Vector3 newPos = pair.Key * Random.Range(0.9f, 1.1f);

            foreach(int i in pair.Value)
            {
                _verts[i] = newPos;
            }
        }

        return _verts;
    }
}
