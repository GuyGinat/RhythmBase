namespace RadialGrid
{
    [System.Serializable]
    public struct RingSegmentID
    {
        public int ringNumber;
        public int segmentNumber;
        
        public RingSegmentID(int ringNumber, int segmentNumber)
        {
            this.ringNumber = ringNumber;
            this.segmentNumber = segmentNumber;
        }
        
        public override string ToString()
        {
            return $"Ring {ringNumber} Segment {segmentNumber}";
        }
    }
}