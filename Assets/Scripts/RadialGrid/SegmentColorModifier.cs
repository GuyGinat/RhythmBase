using UnityEngine;
using UnityEngine.Serialization;

namespace RadialGrid
{
    public class SegmentColorModifier : MonoBehaviour
    {
        
        private Mesh mesh;
        [SerializeField] private Color baseColor;
        [SerializeField] private Color selectionLayerColor;
        [SerializeField] private Color hoverLayerColor;
        private Material material;
        private bool isHovered;
        private bool isSelected;
        private int edgeColorPropertyID;
        
        private void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            edgeColorPropertyID = Shader.PropertyToID("_EdgeColor");
            baseColor = material.GetColor(edgeColorPropertyID);
            selectionLayerColor = baseColor;
            hoverLayerColor = Color.red;
            isHovered = false;
        }

        public void SetColor(Color c)
        {
            material.SetColor(edgeColorPropertyID, c);
        }
        
        public void SetSelected(Color color, Color hoverColor)
        {
            isSelected = true;
            selectionLayerColor = color;
            hoverLayerColor = hoverColor;
            SetColor(selectionLayerColor);
        }

        public void Deselect()
        {
            isSelected = false;
            hoverLayerColor = Color.red;
            SetColor(baseColor);
        }
        
        public void OnHoverExit()
        {
            if (!isHovered) return;
            isHovered = false;
            SetColor(isSelected ? selectionLayerColor : baseColor);
            // material.DOColor(previousColor, 0.1f);
        }

        public void OnHoverEnter()
        {
            if (isHovered) return;
            isHovered = true;
            SetColor(hoverLayerColor);
            // material.DOColor(Color.red, 0.1f);
        }
    }
}