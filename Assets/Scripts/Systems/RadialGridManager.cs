using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RadialGrid;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Systems
{
    public class RadialGridManager : MonoBehaviourSingleton<RadialGridManager>
    {
        [Header("Rings & Segments")]
        public int totalNumberOfSegments;
        public List<Ring> rings;
        [PropertySpace(SpaceAfter = 20)]
        public List<RingSegment> segments;
        
        [Header("Distance")]
        public int distanceToSegmentOnRing;
        public int distanceToSegmentOnNextRing;
        
        
        #region Getter Methods
        
        [TabGroup("Getter Methods")]
        [Button]
        public void SetSegments()
        {
            segments = new List<RingSegment>();
            foreach (Ring ring in rings)
            {
                segments.AddRange(ring.segments);
            }
        }

        [TabGroup("Getter Methods")]
        [Button]
        public void AddConvexMeshColliderToSegments()
        {
            foreach (RingSegment segment in segments)
            {
                var collider = segment.gameObject.AddComponent<MeshCollider>();
                collider.convex = true;
            }
        }
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegment GetSegment(int ringNumber, int segmentNumber)
        {
            return rings[ringNumber].segments[segmentNumber];
        }
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegment GetSegment(RingSegmentID id)
        {
            foreach (Ring ring in rings)
            {
                if (ring.ringNumber == id.ringNumber)
                {
                    foreach (RingSegment ringSegment in ring.segments)
                    {
                        if (ringSegment.segmentNumber == id.segmentNumber)
                        {
                            return ringSegment;
                        }
                    }
                }
            }
            return null;
        }
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID[] GetSegmentsBehind(RingSegmentID id)
        {
            int ringNumber = id.ringNumber;
            int segmentNumber = id.segmentNumber;
            
            List<RingSegmentID> segments = new List<RingSegmentID>();
            int i = ringNumber + 1;
            if (i >= rings.Count) return segments.ToArray();
            segments.Add(new RingSegmentID(i, (segmentNumber * 2) % rings[i].segmentCount));
            segments.Add(new RingSegmentID(i, (segmentNumber * 2 + 1) % rings[i].segmentCount));
            
            return segments.ToArray();
        }
        
        public bool TryGetSegmentsBehind(RingSegmentID id, out RingSegmentID[] segmentIds)
        {
            if (id.ringNumber == rings.Count - 1)
            {
                segmentIds = new RingSegmentID[0];
                return false;
            }
            segmentIds = GetSegmentsBehind(id);
            return true;
        }
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID[] GetAllSegmentsBehind(RingSegmentID id)
        {
            List<RingSegmentID> segments = new List<RingSegmentID>();
            RingSegmentID currentSegment = id;
            
            if (TryGetSegmentsBehind(id, out RingSegmentID[] segmentsBehind))
            {
                segments.AddRange(segmentsBehind);
                foreach (RingSegmentID segmentId in segmentsBehind)
                {
                    segments.AddRange(GetAllSegmentsBehind(segmentId)); // recursive call to get all segments behind the current
                }
            }
            return segments.ToArray();
        }
        
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID GetSegmentInFront(RingSegmentID id)
        {
            int ringNumber = id.ringNumber;
            int segmentNumber = id.segmentNumber;
            return new RingSegmentID(ringNumber - 1, (segmentNumber / 2) % rings[ringNumber - 1].segmentCount);
            
        }
        
        public bool TryGetRingSegmentInFront(RingSegmentID id, out RingSegmentID segmentId)
        {
            if (id.ringNumber == 0)
            {
                segmentId = new RingSegmentID(0, 0);
                return false;
            }
            segmentId = GetSegmentInFront(id);
            return true;
        }

        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID GetOpposingSegment(RingSegmentID id)
        {
            int ringNumber = id.ringNumber;
            int segmentNumber = id.segmentNumber;
            int ringSize = rings[ringNumber].segmentCount;
            
            return segmentNumber < ringSize / 2
                ? new RingSegmentID(ringNumber, segmentNumber + ringSize / 2)
                : new RingSegmentID(ringNumber, segmentNumber - ringSize / 2);
            ;
        }

        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID[] GetLineOfSegmentsUntilOpposing(RingSegmentID id)
        {
            List<RingSegmentID> segments = new List<RingSegmentID>();
            int ringNumber = id.ringNumber;
            int segmentNumber = id.segmentNumber;
            int ringSize = rings[ringNumber].segmentCount;
            segments.Add(GetOpposingSegment(id));
            
            RingSegmentID currentSegment = id;
            while (currentSegment.ringNumber > 0)
            {
                RingSegmentID inFrontOfSegment = GetRingSegmentIdInFrontOfSegment(currentSegment);
                RingSegmentID opposingSegment = GetOpposingSegment(inFrontOfSegment);
                segments.Add(inFrontOfSegment);
                segments.Add(opposingSegment);
                currentSegment = inFrontOfSegment;
            }

            return segments.ToArray();
        }
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID GetRingSegmentIdInFrontOfSegment(RingSegmentID id, bool getInFirstRing = true)
        {
            int ringNumber = id.ringNumber;
            int segmentNumber = id.segmentNumber;
            int ringSize = rings[ringNumber].segmentCount;
            // for (0, 0) and (0, 1) return (0, 2) and (0, 3) 
            if (ringNumber == 0 && getInFirstRing)
            {
                return segmentNumber < ringSize / 2 ?
                    new RingSegmentID(0, segmentNumber + ringSize / 2) :
                    new RingSegmentID(0, segmentNumber - ringSize / 2);
            }
            int nextRing = Mathf.Max(0, ringNumber - 1);
            return new RingSegmentID(nextRing, (segmentNumber / 2) % rings[nextRing].segmentCount);
        }
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID[] GetAdjacentRingSegmentIDs(RingSegmentID id)
        {
            List<RingSegmentID> segmentIds = new List<RingSegmentID>();
            int ringNumber = id.ringNumber;
            int segmentNumber = id.segmentNumber;
            int ringSize = rings[ringNumber].segmentCount;
            
            segmentIds.Add(GetNextSegmentIdInRing(id));
            segmentIds.Add(GetPreviousSegmentIdInRing(id));
            
            if (TryGetSegmentsBehind(id, out RingSegmentID[] segmentsBehind))
            {
                segmentIds.AddRange(segmentsBehind);
            }
            if (TryGetRingSegmentInFront(id, out RingSegmentID segmentInFront))
            {
                segmentIds.Add(segmentInFront);
            }
            
            return segmentIds.ToArray();
        }
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID GetNextSegmentIdInRing(RingSegmentID id)
        {
            return new RingSegmentID(id.ringNumber, (id.segmentNumber + 1) % rings[id.ringNumber].segmentCount);
        }
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID GetPreviousSegmentIdInRing(RingSegmentID id)
        {
            int ringNumber = id.ringNumber;
            int segmentNumber = id.segmentNumber;
            int ringSize = rings[ringNumber].segmentCount;
            return new RingSegmentID(id.ringNumber, segmentNumber == 0 ? ringSize - 1 : segmentNumber - 1);
        }
        
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID GetRandomSegmentInRing(int ringNumber)
        {
            return new RingSegmentID(ringNumber, Random.Range(0, rings[ringNumber].segmentCount));
        }
        
        
        public RingSegmentID[] GetSegmentIdsInDistanceFromSegmentRecursive(RingSegmentID id, int distance)
        {
            List<RingSegmentID> segmentIds = new List<RingSegmentID>();
            segmentIds.Add(id);
            if (distance <= 0)
            {
                return segmentIds.ToArray();
            }
            
            if (distance >= distanceToSegmentOnRing)
            {
                RingSegmentID next = GetNextSegmentIdInRing(id);
                RingSegmentID previous = GetPreviousSegmentIdInRing(id);
                segmentIds.AddRange(GetSegmentIdsInDistanceFromSegmentRecursive(next, distance - distanceToSegmentOnRing));
                segmentIds.AddRange(GetSegmentIdsInDistanceFromSegmentRecursive(previous, distance - distanceToSegmentOnRing));
            }
            if (distance >= distanceToSegmentOnNextRing)
            {
                RingSegmentID next = GetRingSegmentIdInFrontOfSegment(id, false);
                RingSegmentID[] previous = GetSegmentsBehind(id);
                segmentIds.AddRange(GetSegmentIdsInDistanceFromSegmentRecursive(next, distance - distanceToSegmentOnNextRing));
                foreach (RingSegmentID segmentId in previous)
                {
                    segmentIds.AddRange(GetSegmentIdsInDistanceFromSegmentRecursive(segmentId, distance - distanceToSegmentOnNextRing));
                }
            }
            
            return segmentIds.ToArray();
        }

        // returns all segments in the ring and the next ring based on distance thresholds set in the inspector
        [TabGroup("Getter Methods")]
        [Button]
        public RingSegmentID[] GetSegmentIdsInDistanceFromSegment(RingSegmentID id, int distance)
        {
            return GetSegmentIdsInDistanceFromSegmentRecursive(id, distance).Distinct().ToArray();
        }
        
        #endregion
        
        
        #region Rings & Segments Effects
        
        [TabGroup("Rings & Segments Effects")]
        [Button]
        public void RotateRing(int ringNumber, int segmentsToRotate = 1)
        {
            print($"Rotating ring {ringNumber} by {segmentsToRotate} segments");
            Ring ring = rings[ringNumber];
            float rotation = 360f / ring.segmentCount;
            ring.transform.DORotate(new Vector3(0f, ring.transform.eulerAngles.y + rotation * -segmentsToRotate, 0f), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            foreach (RingSegment ringSegment in ring.segments)
            {
                int segmentNumber = (ringSegment.segmentNumber + segmentsToRotate) % ring.segmentCount;
                if (segmentNumber < 0)
                {
                    segmentNumber = ring.segmentCount + segmentNumber;
                }
                ringSegment.SetSegment(ringNumber, segmentNumber);
            }           
        }
        
        [TabGroup("Rings & Segments Effects")]
        [Button]
        public void RaiseSegments(RingSegmentID[] ids)
        {
            print("Raising segments");
            foreach (RingSegmentID id in ids)
            {
                RaiseSegment(id);
            }
        }
        
        [TabGroup("Rings & Segments Effects")]
        [Button]
        public void RaiseSegment(RingSegmentID id)
        {
            print("Raising segment");
            RingSegment segment = GetSegment(id);
            segment.transform.DOLocalMoveY(3f, 2.5f).SetEase(Ease.Linear);
            segment.transform.DOLocalMoveY(0f, 2f).SetEase(Ease.Linear).SetDelay(5f);
        }
        
        [TabGroup("Rings & Segments Effects")]
        [Button]
        public void MarkSegments(RingSegmentID[] ids)
        {
            foreach (RingSegmentID id in ids)
            {
                GetSegment(id).MarkSegment();
            }
        }
        
        [TabGroup("Rings & Segments Effects")]
        [Button]
        public void MarkSegments(RingSegmentID[] ids, Color color)
        {
            foreach (RingSegmentID id in ids)
            {
                GetSegment(id).MarkSegment(color);
            }
        }
        
        [TabGroup("Rings & Segments Effects")]
        [Button]
        public void SelectSegments(RingSegmentID[] ids, Color color, Color hoverColor)
        {
            foreach (RingSegmentID id in ids)
            {
                GetSegment(id).SetSelected(color, hoverColor);
            }
        }
        
        [TabGroup("Rings & Segments Effects")]
        [Button]
        public void DeselectSegments(RingSegmentID[] ids)
        {
            foreach (RingSegmentID id in ids)
            {
                GetSegment(id).Deselect();
            }
        }
        
        
        [TabGroup("Rings & Segments Effects")]
        [Button]
        public void UnmarkSegments(RingSegmentID[] ids)
        {
            foreach (RingSegmentID id in ids)
            {
                GetSegment(id).UnmarkSegment();
            }
        }
        
        [TabGroup("Rings & Segments Effects")]
        [Button]
        public void UnmarkAllSegments()
        {
            foreach (RingSegment segment in segments)
            {
                segment.UnmarkSegment();
            }
        }
        
        #endregion

        public Vector3 GetPosition(int ringNumber, int segmentNumber)
        {
            return rings[ringNumber].segments[segmentNumber].Position;
        }

    }
}