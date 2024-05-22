using System.Collections;
using System.Collections.Generic;
using RadialGrid;
using Songs;
using Systems;
using UnityEngine;

public class CustomNoteResponder : MonoBehaviour
{
    
    public RingSegmentID ringSegmentIDToStartFrom; 
    public RingSegmentID currentRingSegmentID;
    
    public Note.DrumNoteType currentNoteType;
    
    public List<EffectSequenceItem> effectSequence;
    public Color effectColor = Color.red;
    private EffectSequenceItem currentEffectSequenceItem;
    private int currentEffectSequenceIndex = 0;
    
    
    private void Start()
    {
        currentRingSegmentID = ringSegmentIDToStartFrom;
        currentEffectSequenceItem = effectSequence[0];
    }

    public void OnNotePlayed()
    {
        RingSegmentID ringSegmentID = TriggerEffectItem(currentEffectSequenceItem);
        currentEffectSequenceItem = effectSequence[currentEffectSequenceIndex++ % effectSequence.Count];
        
        var ringSegment = RadialGridManager.Instance.GetSegment(ringSegmentID);
        currentRingSegmentID = ringSegmentID;
        print(ringSegment.segmentNumber);
        // currentRingSegmentID = ringSegment;
        // RadialGridManager.Instance.GetSegment(currentRingSegmentID.ringNumber, currentRingSegmentID.segmentNumber).MarkSegment();
        RadialGridManager.Instance.GetSegment(currentRingSegmentID.ringNumber, currentRingSegmentID.segmentNumber).MarkAndResetSegment(effectColor);
    }

    public RingSegmentID TriggerEffectItem(EffectSequenceItem item)
    {
        RingSegmentID ringSegmentID = currentRingSegmentID;
        switch (item)
        {
            case EffectSequenceItem.NextSegmentOnRing:
                ringSegmentID = RadialGridManager.Instance.GetNextSegmentIdInRing(currentRingSegmentID);
                break;
            case EffectSequenceItem.PreviousSegmentOnRing:
                ringSegmentID = RadialGridManager.Instance.GetPreviousSegmentIdInRing(currentRingSegmentID);
                break;
            case EffectSequenceItem.NextRing:
                ringSegmentID = RadialGridManager.Instance.GetSegmentInFront(currentRingSegmentID);
                break;
            case EffectSequenceItem.PreviousRing:
                ringSegmentID = RadialGridManager.Instance.GetSegmentsBehind(currentRingSegmentID)[0];
                break;
            case EffectSequenceItem.RandomSegmentOnRing:
                ringSegmentID = RadialGridManager.Instance.GetRandomSegmentInRing(currentRingSegmentID.ringNumber);
                break;
            case EffectSequenceItem.FullRing:
                break;
            case EffectSequenceItem.NextFullRing:
                break;
            default:
                break;
        }

        return ringSegmentID;
    }
}

public enum EffectSequenceItem
{
    NextSegmentOnRing,
    PreviousSegmentOnRing,
    NextRing,
    PreviousRing,
    RandomSegmentOnRing,
    FullRing,
    NextFullRing,
}
