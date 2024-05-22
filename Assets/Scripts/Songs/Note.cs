namespace Songs
{
    [System.Serializable]
    public class Note
    {
        public enum GuitarNoteType
        {
            Green,  
            Red,    
            Yellow, 
            Blue,   
            Orange,
            OpenNote,
            StrumHOPOModifier,
            TapModifier,        // overrides strum
            Unrecognized
        }
        public enum DrumNoteType
        {
            Kick,
            Red,
            Yellow,
            Blue,
            FiveLaneOrangeFourLaneGreen,
            FiveLaneGreen,
            DoubleKick,
            RedAccent,
            YellowAccent,
            BlueAccent,
            FiveLaneOrangeFourLaneGreenAccent,
            FiveLaneGreenAccent,
            RedGhost,
            YellowGhost,
            BlueGhost,
            FiveLaneOrangeFourLaneGreenGhost,
            FiveLaneGreenGhost,
            YellowCymbal,
            BlueCymbal,
            GreenCymbal,
            Unrecognized
        }

        public float tick;
        public GuitarNoteType guitarNoteType;
        public DrumNoteType drumNoteType;
        public bool isDrumNote;
        public bool isGuitarNote;
        public float noteDuration;

        public static GuitarNoteType GetNoteType(int key)
        {
            switch (key)
            {
                case 0: return GuitarNoteType.Green;
                case 1: return GuitarNoteType.Red;
                case 2: return GuitarNoteType.Yellow;
                case 3: return GuitarNoteType.Blue;
                case 4: return GuitarNoteType.Orange;
                case 7: return GuitarNoteType.OpenNote;
                case 5: return GuitarNoteType.StrumHOPOModifier;
                case 6: return GuitarNoteType.TapModifier;
                default: return GuitarNoteType.Unrecognized; // Default case if note key is unrecognized
            }
        }
    
        public static DrumNoteType GetDrumNoteType(int key)
        {
            switch (key)
            {
                case 0: return DrumNoteType.Kick;
                case 1: return DrumNoteType.Red;
                case 2: return DrumNoteType.Yellow;
                case 3: return DrumNoteType.Blue;
                case 4: return DrumNoteType.FiveLaneOrangeFourLaneGreen;
                case 5: return DrumNoteType.FiveLaneGreen;
                case 32: return DrumNoteType.DoubleKick;
                case 34: return DrumNoteType.RedAccent;
                case 35: return DrumNoteType.YellowAccent;
                case 36: return DrumNoteType.BlueAccent;
                case 37: return DrumNoteType.FiveLaneOrangeFourLaneGreenAccent;
                case 38: return DrumNoteType.FiveLaneGreenAccent;
                case 40: return DrumNoteType.RedGhost;
                case 41: return DrumNoteType.YellowGhost;
                case 42: return DrumNoteType.BlueGhost;
                case 43: return DrumNoteType.FiveLaneOrangeFourLaneGreenGhost;
                case 44: return DrumNoteType.FiveLaneGreenGhost;
                case 66: return DrumNoteType.YellowCymbal;
                case 67: return DrumNoteType.BlueCymbal;
                case 68: return DrumNoteType.GreenCymbal;
                default: return DrumNoteType.Unrecognized;
            }
        }
    }
}