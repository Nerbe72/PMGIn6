using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPlanet : MonoBehaviour
{
    private struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int _v1, int _v2, int _v3)
        {
            this.v1 = _v1;
            this.v2 = _v2;
            this.v3 = _v3;
        }
    }

    private Mesh mesh;

    public bool randomize;
    public bool offset;
    public bool invert;
    public float rad;
    public int detail;
    public MeshCollider meshCollider;

    private void Start()
    {
        Create(rad, detail);

        if (offset)
        {
            Offset();
        }

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh;
        }
    }

    private void Create(float _radius, int _recursionLevel)
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        mesh = filter.mesh;
        mesh.Clear();

        List<Vector3> vertList = new List<Vector3>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

        //정이십면체를 이루는 정점 12개를 만듦
        float t = (1f + Mathf.Sqrt(5f) / 2f);

        vertList.Add(new Vector3(-1f, t, 0f).normalized * _radius);
        vertList.Add(new Vector3(1f, t, 0f).normalized * _radius);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized * _radius);
        vertList.Add(new Vector3(1f, -t, 0f).normalized * _radius);


        vertList.Add(new Vector3(0, -1f, t).normalized * _radius);
        vertList.Add(new Vector3(0, 1f, t).normalized * _radius);
        vertList.Add(new Vector3(0, -1f, -t).normalized * _radius);
        vertList.Add(new Vector3(0, 1f, -t).normalized * _radius);


        vertList.Add(new Vector3(t, 0f, -1f).normalized * _radius);
        vertList.Add(new Vector3(t, 0f, 1f).normalized * _radius);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized * _radius);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized * _radius);

        //정이십면체를 이루는 삼각형 20개를 만듦
        List<TriangleIndices> faces = new List<TriangleIndices>();

        //포인트 0주변 5개 페이스
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));

        //
        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));

        //
        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));

        //
        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));

        //
        for(int i = 0; i < _recursionLevel; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();

            foreach(var tri in faces)
            {
                int a = GetMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, _radius);
                int b = GetMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, _radius);
                int c = GetMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, _radius);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        mesh.vertices = vertList.ToArray();

        List<int> triList = new List<int>();
        for (int i = 0; i < faces.Count; i++)
        {
            triList.Add(faces[i].v1);
            triList.Add(faces[i].v2);
            triList.Add(faces[i].v3);
        }

        mesh.triangles = triList.ToArray();
        mesh.uv = new Vector2[mesh.vertices.Length];

        Vector3[] normales = new Vector3[vertList.Count];
        for (int i = 0; i < normales.Length; i++)
            normales[i] = vertList[i].normalized;


        mesh.normals = normales;

        if (invert)
        {
            // Reverse the triangles
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int j = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = j;
            }
            mesh.triangles = triangles;

            // Reverse the normals;
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;
        }

        mesh.RecalculateBounds();
        mesh.Optimize();

        if (randomize)
            mesh.vertices = Randomize(mesh.vertices);
    }

    private void Offset()
    {
        int offset = Random.Range(1, 7);
        int offsetMod = Random.Range(0, 2);

        if (offsetMod == 0)
        {
            offsetMod = -1;
        }

        Vector3 offsetVec = new Vector3();

        switch (offset)
        {
            case 1:
                offsetVec = new Vector3(Random.Range(0.01f, 0.05f), 0, 0) * offsetMod;
                break;
            case 2:
                offsetVec = new Vector3(0, Random.Range(0.01f, 0.05f), 0) * offsetMod;
                break;
            case 3:
                offsetVec = new Vector3(0, 0, Random.Range(0.01f, 0.05f)) * offsetMod;
                break;
        }

        transform.position = transform.position += offsetVec;
    }

    private int GetMiddlePoint(int _p1, int _p2, ref List<Vector3> _vertices, ref Dictionary<long, int> _cache, float _radius)
    {
        bool firstIsSmaller = _p1 < _p2;
        long smallerIndex = firstIsSmaller ? _p1 : _p2;
        long greaterIndex = firstIsSmaller ? _p2 : _p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (_cache.TryGetValue(key, out ret))
        {
            return ret;
        }

        //캐시에 없으면 계산
        Vector3 point1 = _vertices[_p1];
        Vector3 point2 = _vertices[_p2];
        Vector3 middle = new Vector3((point1.x + point2.x) / 2f, (point1.y + point2.y) / 2f, (point1.z + point2.z) / 2f);

        //
        int i = _vertices.Count;
        _vertices.Add(middle.normalized * _radius);

        _cache.Add(key, i);

        return i;
    }

    Vector3[] Randomize(Vector3[] _verts)
    {

        for (int x = 0; x < _verts.Length; x++)
        {
            Vector3 newPos = _verts[x] * Random.Range(0.95f, 1.03f);
            _verts[x] = newPos;
        }

        return _verts;
    }
}
