using System.Collections.Generic;
using UnityEngine;

namespace RadialGrid
{
    [System.Serializable]
    public class Ring : MonoBehaviour
    {
        public int ringNumber;
        public int segmentCount;
        public List<RingSegment> segments;
    
        public void SetRing(int ringNumber, int segmentCount)
        {
            this.ringNumber = ringNumber;
            this.segmentCount = segmentCount;
            segments = new List<RingSegment>();
        }
    
        public void AddSegment(RingSegment segment)
        {
            segments.Add(segment);
        }
    }
}
