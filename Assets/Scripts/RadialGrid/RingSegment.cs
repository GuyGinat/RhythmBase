using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using Systems;
using UnityEditor;
using UnityEngine;

namespace RadialGrid
{
    [RequireComponent(typeof(SegmentColorModifier))]
    public class RingSegment : MonoBehaviour
    {
        public int ringNumber;
        public int segmentNumber;
        public bool isFlat = false;
        
        public Vector3 position;
        public Vector3 Position => CalculateMeshWorldCenter();
    
        private Mesh mesh;
        [SerializeField] private Color startColor;
        [SerializeField] private Color previousColor;
        private Material material;

        public int edgeColorPropertyID;
        public int fillColorPropertyID; 
        
        private bool isHovered;
        private SegmentColorModifier colorModifier;
        public RingSegmentID segmentID => new RingSegmentID(ringNumber, segmentNumber);
        
        private void Start()
        {
            EventManager.Instance.OnRadialGridCreated += OnRadialGridCreated;
            colorModifier = GetComponent<SegmentColorModifier>();
            material = GetComponent<MeshRenderer>().material;
            edgeColorPropertyID = Shader.PropertyToID("_EdgeColor");
            fillColorPropertyID = Shader.PropertyToID("_FillColor");
            startColor = material.GetColor(edgeColorPropertyID);
            isHovered = false;
            previousColor = startColor;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnRadialGridCreated -= OnRadialGridCreated;
        }

        private void OnRadialGridCreated()
        {
            CalculateMeshWorldCenter();
        }

        public void SetSegment(int ringNumber, int segmentNumber, bool isFlat = false)
        {
            this.ringNumber = ringNumber;
            this.segmentNumber = segmentNumber;
            this.isFlat = isFlat;
        }
        
        private void OnDrawGizmos()
        {
            if (position == Vector3.zero) return;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Position, 0.1f);
            Handles.Label(Position, $"({ringNumber}, {segmentNumber})");
        }

        [Button]
        Vector3 CalculateMeshWorldCenter()
        {
            if (mesh == null) mesh = GetComponent<MeshFilter>()?.sharedMesh;
            if (mesh == null) return Vector3.zero;
            
            Vector3[] vertices = mesh.vertices;
            Vector3 sum = Vector3.zero;

            foreach (Vector3 vertex in vertices)
            {
                sum += vertex;
            }

            Vector3 center = sum / vertices.Length + transform.root.position;

            // Convert the center from local to world space
            position = new Vector3(center.x, 0f, center.z);
            return position;
        }

        public void SetSelected(Color c, Color hoverColor)
        {
            colorModifier.SetSelected(c, hoverColor);
        }

        public void Deselect()
        {
            colorModifier.Deselect();
        }

        public void MarkSegment()
        {
            material.SetColor(edgeColorPropertyID, Color.red);
            previousColor = material.color;
            // material.DOColor(Color.red, 0.1f);
        }
        
        public void MarkAndResetSegment() => MarkAndResetSegment(Color.red);
        public void MarkAndResetSegment(Color color)
        {
            material.SetColor(edgeColorPropertyID, color);
            previousColor = material.color;
            // material.DOColor(Color.red, 0.1f);
            
            DOVirtual.DelayedCall(0.1f, () => material.SetColor(edgeColorPropertyID, startColor));
        }
        
        
        
        
        public void MarkSegment(Color color)
        {
            previousColor = material.GetColor(edgeColorPropertyID);
            material.SetColor(edgeColorPropertyID, color);
            // material.DOColor(color, 0.1f);
        }
        
        public void UnmarkSegment()
        {
            material.SetColor(edgeColorPropertyID, previousColor);
            // material.DOColor(previousColor, 0.1f);
        }

        public void OnHoverExit()
        {
            colorModifier.OnHoverExit();
            return;
            if (!isHovered) return;
            isHovered = false;
            material.SetColor(edgeColorPropertyID, previousColor);
            // material.DOColor(previousColor, 0.1f);
        }

        public void OnHoverEnter()
        {
            colorModifier.OnHoverEnter();
            return;
            if (isHovered) return;
            isHovered = true;
            material.SetColor(edgeColorPropertyID, Color.red);
            // material.DOColor(Color.red, 0.1f);
        }
        
    }
}