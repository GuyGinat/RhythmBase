using RadialGrid;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.DebugTools
{
    public class UIDebugTool : MonoBehaviour
    {
    
        public Slider rotationSlider;
        public TextMeshProUGUI rotationText;
        public Slider RingNumberSlider;
        public TextMeshProUGUI RingNumberText;
    
        public TMP_InputField ringNumberInput;
        public TMP_InputField segmentNumberInput;
    
        // Start is called before the first frame update
        void Start()
        {
            rotationSlider.onValueChanged.AddListener(OnRotationSliderChanged);
            RingNumberSlider.onValueChanged.AddListener(OnRingNumberSliderChanged);
        
            EventManager.Instance.OnRadialGridCreated += OnRadialGridCreated;
        }
    
        private void OnDisable()
        {
            EventManager.Instance.OnRadialGridCreated -= OnRadialGridCreated;
        }

        private void OnRadialGridCreated()
        {
            // RingNumberSlider.maxValue = RadialGridManager.Instance.rings.Count - 1;
        }

        public void RaiseSegments()
        {
            RadialGridManager radialGridMeshGenerator = FindObjectOfType<RadialGridManager>();
            int ringNumber = int.Parse(ringNumberInput.text);
            int segmentNumber = int.Parse(segmentNumberInput.text);
            var segments= radialGridMeshGenerator.GetSegmentsBehind(new RingSegmentID(ringNumber, segmentNumber));
            radialGridMeshGenerator.RaiseSegments(segments);
        }
    
        private void OnRingNumberSliderChanged(float val)
        {
            int ringNumber = (int) val;
            RadialGridManager radialGridMeshGenerator = FindObjectOfType<RadialGridManager>();
            int numberOfSegments = radialGridMeshGenerator.rings[ringNumber].segments.Count;
            rotationSlider.maxValue = numberOfSegments;
            rotationSlider.minValue = -numberOfSegments;
            RingNumberText.text = "Ring Number " + val;
        }

        private void OnRotationSliderChanged(float val)
        {
            rotationText.text = "Rotations " + val;
        }

        public void Rotate()
        {
            RadialGridManager radialGridMeshGenerator = FindObjectOfType<RadialGridManager>();
            radialGridMeshGenerator.RotateRing((int)RingNumberSlider.value, (int)rotationSlider.value);
        }
    }
}
