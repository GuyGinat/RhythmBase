using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Systems;
using UnityEngine;
using Utils;

namespace RadialGrid
{
    public class RadialGridMeshGenerator : MonoBehaviourSingleton<RadialGridMeshGenerator>
    {
        [Header("Settings")]
        public int segmentCount = 10;
        public int ringCount = 3;
        public float innerRadius = 1f;
        public float outerRadius = 2f;
        public int initialRingDivisions = 4;
        public int ringDivisionsMultiplier = 2;
        public float ringsOffset;
        [ReadOnly]
        public int totalNumberOfSegments;

        [Header("Materials")]
        public Material material;
        public Material material2;
        
        [Header("Generated Rings")]
        [ReadOnly]
        public List<Ring> rings;

        private void Start()
        {
            GenerateRings();
        }

        #region Generation

        [Button]
        public void GenerateRings()
        {
            rings = new List<Ring>();
            int currentRingDivisions = initialRingDivisions;
            float currentInnerRadius = innerRadius;
            float currentOuterRadius = outerRadius;
            for (int i = 0; i < ringCount; i++)
            {
                Ring ring = (GenerateRing(currentInnerRadius, currentOuterRadius, currentRingDivisions, i));
                ring.transform.SetParent(transform);
                rings.Add(ring);
                currentRingDivisions *= ringDivisionsMultiplier;
                currentInnerRadius = currentOuterRadius + ringsOffset;
                currentOuterRadius = currentInnerRadius + outerRadius;
            }
            EventManager.Instance.RadialGridCreated();
        }

        public Ring GenerateRing(float startRadius, float endRadius, int ringDivisions, int ringNumber)
        {
            
            GameObject ringGameObject = new GameObject($"Ring {ringNumber}");
            Ring ring = ringGameObject.AddComponent<Ring>();
            ring.SetRing(ringNumber, ringDivisions);
            
            List<GameObject> segments = new List<GameObject>();
            float angleStep = 360f / ringDivisions;
            for (int i = 0; i < ringDivisions; i++)
            {
                
                var a = GenerateRingSegment2D(startRadius, endRadius, angleStep * i, angleStep * (i + 1), i, segmentCount);
                // List<Vector3> vertices = a.vertices;
                var segment = Generate3DSegmentMesh(a.vertices, a.uvs, ringNumber, i);
                segment.transform.SetParent(ringGameObject.transform);
                ring.AddSegment(segment.GetComponent<RingSegment>());
                // break;
            }

            return ring;
        }
        
        public (List<Vector3> vertices, List<Vector2> uvs) GenerateRingSegment2D(float startRadius, float endRadius, float startAngle, float endAngle, int segmentNumber, int detailLevel)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            float angleStep = (endAngle - startAngle) / detailLevel;
            for (int i = 0; i < detailLevel + 1; i++)
            {
                float angle = i * angleStep + startAngle;
                Vector3 pos = transform.position + GetUnitVector(angle) * startRadius;
                Vector3 pos2 = transform.position + GetUnitVector(angle) * endRadius;
                vertices.Add(pos);
                vertices.Add(pos2);
                uvs.Add(new Vector2(0, (float)i / detailLevel));
                uvs.Add(new Vector2(1, (float)i / detailLevel));
            }
            return (vertices, uvs);
        }

        public List<Vector2> CreateNormalizedUVs(List<Vector3> vertices)
        {
            List<Vector2> uvs = new List<Vector2>();

            Bounds bounds = new Bounds();
            foreach (var vertex in vertices)
            {
                bounds.Encapsulate(vertex);
            }

            return uvs;
        }

        // public GameObject Generate2DSegmentMesh(List<Vector3> vertices, int ringNumber, int segmentNumber)
        // {
        //     GameObject go = new GameObject($"2D Segment {ringNumber}-{segmentNumber}");
        //     go.AddComponent<RingSegment>().SetSegment(ringNumber, segmentNumber, true);
        //     
        //     var meshFilter = go.AddComponent<MeshFilter>();
        //     var meshRenderer = go.AddComponent<MeshRenderer>();
        //     meshRenderer.material = segmentNumber % 2 == 0 ? material : material2;
        //     
        //     Mesh mesh = new Mesh();
        //     int numVerticesIn2D = vertices.Count / 2;
        //
        //     List<Vector2> uvs = new List<Vector2>();
        //     List<int> trianglesTop = new List<int>();
        //     // Top
        //     for (int i = 0; i < numVerticesIn2D - 2; i += 2) {
        //         trianglesTop.Add(i + 2);
        //         trianglesTop.Add(i + 1);
        //         trianglesTop.Add(i);
        //         
        //         trianglesTop.Add(i + 2);
        //         trianglesTop.Add(i + 3);
        //         trianglesTop.Add(i + 1);
        //     }
        //     
        // }
        //
        public GameObject Generate3DSegmentMesh(List<Vector3> vertices, List<Vector2> uvs, int ringNumber, int segmentNumber)
        {
            GameObject go = new GameObject($"Segment {ringNumber}-{segmentNumber}");
            go.AddComponent<RingSegment>().SetSegment(ringNumber, segmentNumber);
            
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.material = segmentNumber % 2 == 0 ? material : material2;
            List<Vector3> verticesBottom = vertices.Select(v => v + Vector3.down * 1f).ToList();
            vertices.AddRange(verticesBottom);
            uvs.AddRange(uvs);
            Mesh mesh = new Mesh();
            
            
            int numVerticesIn2D = vertices.Count / 2;

            List<int> trianglesTop = new List<int>();
            List<int> trianglesBottom = new List<int>();
            // Top
            for (int i = 0; i < numVerticesIn2D - 2; i += 2) {
                trianglesTop.Add(i + 2);
                trianglesTop.Add(i + 1);
                trianglesTop.Add(i);
                
                trianglesTop.Add(i + 2);
                trianglesTop.Add(i + 3);
                trianglesTop.Add(i + 1);
                
                trianglesBottom.Add(i + numVerticesIn2D);
                trianglesBottom.Add(i + 1 + numVerticesIn2D);
                trianglesBottom.Add(i + 2 + numVerticesIn2D);
                
                trianglesBottom.Add(i + 1 + numVerticesIn2D);
                trianglesBottom.Add(i + 3 + numVerticesIn2D);
                trianglesBottom.Add(i + 2 + numVerticesIn2D);
            }
            List<int> sideTriangles = new List<int>();
            sideTriangles.Add(0);
            sideTriangles.Add(1);
            sideTriangles.Add(numVerticesIn2D);
            
            sideTriangles.Add(1);
            sideTriangles.Add(numVerticesIn2D + 1);
            sideTriangles.Add(numVerticesIn2D);
            
            sideTriangles.Add(numVerticesIn2D - 2);
            sideTriangles.Add(numVerticesIn2D - 1);
            sideTriangles.Add(numVerticesIn2D * 2 - 2);
            
            sideTriangles.Add(numVerticesIn2D - 1);
            sideTriangles.Add(numVerticesIn2D * 2 - 1);
            sideTriangles.Add(numVerticesIn2D * 2 - 2);
            
            for (int i = 0; i < numVerticesIn2D - 2; i += 2)
            {
                sideTriangles.Add(i);
                sideTriangles.Add(i + 2);
                sideTriangles.Add(i + numVerticesIn2D);
                
                sideTriangles.Add(i + 2);
                sideTriangles.Add(i + numVerticesIn2D + 2);
                sideTriangles.Add(i + numVerticesIn2D);
                
                sideTriangles.Add(i + 1);
                sideTriangles.Add(i + 3);
                sideTriangles.Add(i + numVerticesIn2D + 1);
                
                sideTriangles.Add(i + 3);
                sideTriangles.Add(i + numVerticesIn2D + 3);
                sideTriangles.Add(i + numVerticesIn2D + 1);
            }
            

            Vector3[] normals = new Vector3[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                // normals[i] = i > vertices.Count / 2 ? Vector3.up : Vector3.down;
                normals[i] = Vector3.up;
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = trianglesTop.ToArray().Concat(trianglesBottom.ToArray()).Concat(sideTriangles.ToArray()).ToArray();
            mesh.normals = normals;
            mesh.uv = uvs.ToArray();
            mesh.RecalculateBounds();
            totalNumberOfSegments++;
            // mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
            
            
            // var outline = go.AddComponent<Outline>();
            // outline.OutlineMode = Outline.Mode.OutlineAll;
            // outline.OutlineColor = Color.black;
            // outline.OutlineWidth = 5f;
            return go;
        }

        private Vector3 CalculateNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vector3 dir = Vector3.Cross(vertex2 - vertex1, vertex3 - vertex1);
            return Vector3.Normalize(dir);
        }
        
        private Vector3 GetUnitVector(float angle)
        {  
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);
            return new Vector3(x, 0f, y);
        }
        #endregion

        public Vector3 GetPosition(int ringNumber, int ringSegmentNumber)
        {
            return rings[ringNumber].segments[ringSegmentNumber].Position;
        }
        
    }
}