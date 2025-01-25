using System.Collections.Generic;
using UnityEngine;

public class ProceduralSphere : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private void Start()
    {
        GenerateSphere(1f, 20, 10);
    }

    private void GenerateSphere(float _radius, int _nbLong, int _nbLat)
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        mesh.Clear();

        #region Vertices
        Vector3[] vertices = new Vector3[(_nbLong + 1) * _nbLat + 2];
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        //북극점
        vertices[0] = Vector3.up * _radius;

        //위도 버텍스
        for(int lat = 0; lat < _nbLat; lat++)
        {
            float latRad = _pi * (float)(lat + 1) / _nbLat; // 북극에서 남극
            float sin1 = Mathf.Sin(latRad); //수평거리(xz)
            float cos1 = Mathf.Cos(latRad); //높이

            //경도 버텍스
            for(int lon = 0; lon <= _nbLong; lon++)
            {
                float lonRad = _2pi * (float)(lon == _nbLong ? 0 : lon) / _nbLong; //경도각도 0 ~ 360 * (n-1)/n
                float sin2 = Mathf.Sin(lonRad); //z축 위치
                float cos2 = Mathf.Cos(lonRad); //x축 위치

                //북극점을 제외하고 시작(1)
                //위도에 해당하는 경도의 시작위치(lat * (nblong + 1))
                //n번째 경도(lon)
                vertices[1 + lat * (_nbLong + 1) + lon] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * _radius;
            }
        }

        //남극점
        vertices[vertices.Length - 1] = Vector3.down * _radius;
        #endregion

        #region Normales
        Vector3[] normales = new Vector3[vertices.Length];

        for(int n = 0; n < vertices.Length; n++)
        {
            normales[n] = vertices[n].normalized;
        }
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        uvs[0] = Vector2.up;
        uvs[uvs.Length - 1] = Vector2.zero;
        for(int lat = 0; lat < _nbLat; lat++)
        {
            for(int lon = 0; lon <= _nbLong; lon++)
            {
                uvs[lon + lat * (_nbLong + 1) + 1] = new Vector2((float)lon / _nbLong, 1f - (float)(lat + 1) / (_nbLat + 1));
            }
        }
        #endregion

        #region Triangles
        int nbFaces = vertices.Length;
        int nbTriangles = nbFaces * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        //Top Cap
        int i = 0;

        for(int lon = 0; lon < _nbLong; lon++)
        {
            triangles[i++] = lon + 2;
            triangles[i++] = lon + 1;
            triangles[i++] = 0;
        }

        //Middle
        for(int lat = 0; lat < _nbLat - 1; lat++)
        {
            for(int lon = 0; lon < _nbLong; lon++)
            {
                int current = lon + lat * (_nbLong + 1) + 1;
                int next = current + _nbLong + 1;

                triangles[i++] = current;
                triangles[i++] = current + 1;
                triangles[i++] = next + 1;

                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = next;

            }
        }

        //Bottom Cap
        for(int lon = 0; lon <= _nbLong; lon++)
        {
            triangles[i++] = vertices.Length - 1;
            triangles[i++] = vertices.Length - (lon + 2) - 1;
            triangles[i++] = vertices.Length - (lon + 1) - 1;
        }
        #endregion

        //mesh.vertices = Randomize(vertices);
        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        mesh.RecalculateBounds();
        mesh.Optimize();
    }

    private Vector3[] Randomize(Vector3[] _verts)
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
                
                for(int j = 0; j < mesh.vertexCount; j++)
                {
                    Debug.Log("Loading");
                }
            }
        }

        return _verts;
    }
}
