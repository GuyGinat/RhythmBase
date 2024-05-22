using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Songs
{
    public static class SongDataLoader
    {
        public static SongData LoadSongData(string songFolder)
        {
            return new SongData(songFolder);
        }
    }
    
    [System.Serializable]
    public class SongData
    {
        public string name;                 // Name of the song
        public string artist;               // Artist of the song
        public string charter;              // Person who charted the song
        public string year;                 // Year the song was released
                                            
        public List<Note> guitarNotes;      // List of guitar notes in the song
        public List<Note> drumsNotes;       // List of drum notes in the song
        public List<Note> bassNotes;        // List of bass notes in the song
        public List<Note> keysNotes;        // List of keys notes in the song
    
        public int resolution;              // How many ticks per beat
    
        public int bpmStart = 0;      // Tick when the current BPM started
        public Dictionary<int, int> ticksToBpmChanges = new Dictionary<int, int>(); // Dictionary of BPM changes
        public int timeSignatureStart = 2;       // Time signature of the song
        public Dictionary<int, int> ticksToTimeSignatureChanges = new Dictionary<int, int>(); // Dictionary of time signature changes
                                          
        public float offset;         // Time when the song starts
        
        private string audioPath;
        private bool hasAudioPath = false;

        private Dictionary<string, bool> hasSectionsDictionary = new Dictionary<string, bool>();
       
    
        #region Sections String Values
        
        private const string SongSection = "[Song]";
        private const string SyncTrackSection = "[SyncTrack]";
        
        private const string GuitarSectionExpert = "[ExpertSingle]";
        private const string GuitarSectionHard = "[HardSingle]";
        private const string GuitarSectionMedium = "[MediumSingle]";
        private const string GuitarSectionEasy = "[EasySingle]";
        
        private const string DrumsSectionExpert = "[ExpertDrums]";
        private const string DrumsSectionHard = "[HardDrums]";
        private const string DrumsSectionMedium = "[MediumDrums]";
        private const string DrumsSectionEasy = "[EasyDrums]";
        
        private const string CoopGuitarSectionExpert = "[ExpertDoubleGuitar]";
        private const string CoopGuitarSectionHard = "[HardDouble]";
        private const string CoopGuitarSectionMedium = "[MediumDouble]";
        private const string CoopGuitarSectionEasy = "[EasyDouble]";
        
        private const string RhythmGuitarSectionExpert = "[ExpertDoubleRhythm]";
        private const string RhythmGuitarSectionHard = "[HardDoubleRhythm]";
        private const string RhythmGuitarSectionMedium = "[MediumDoubleRhythm]";
        private const string RhythmGuitarSectionEasy = "[EasyDoubleRhythm]";
        
        private const string BassSectionExpert = "[ExpertDoubleBass]";
        private const string BassSectionHard = "[HardDoubleBass]";
        private const string BassSectionMedium = "[MediumDoubleBass]";
        private const string BassSectionEasy = "[EasyDoubleBass]";
        
        private const string KeysSectionExpert = "[ExpertKeyboard]";
        private const string KeysSectionHard = "[HardKeyboard]";
        private const string KeysSectionMedium = "[MediumKeyboard]";
        private const string KeysSectionEasy = "[EasyKeyboard]";
        
        #endregion
    
    
        private delegate void ParseSectionLineDelegate(string line);
        
        public SongData(string songFolder)
        {
            LoadSectionVerifierDictionary();
        
            guitarNotes = new List<Note>();
            drumsNotes = new List<Note>();
            bassNotes = new List<Note>();
            keysNotes = new List<Note>();
            
            // Load the song data from the song folder
            string chartPath = Path.Combine(songFolder, "notes.chart");
            if (!File.Exists(chartPath))
            {
                Debug.LogWarning("Chart file not found in " + songFolder);
                return;
            }
            
            string[] lines = File.ReadAllLines(chartPath);
            
        
            ParseSection(lines, SongSection, ParseSongSection);
            ParseSection(lines, SyncTrackSection, ParseSyncTrackSection);
            ParseSection(lines, DrumsSectionExpert, ParseDrumsSection);
        }

        private void LoadSectionVerifierDictionary()
        {
            hasSectionsDictionary = new Dictionary<string, bool>();
            
            hasSectionsDictionary.Add(GuitarSectionExpert, false);
            hasSectionsDictionary.Add(GuitarSectionHard, false);
            hasSectionsDictionary.Add(GuitarSectionMedium, false);
            hasSectionsDictionary.Add(GuitarSectionEasy, false);
            
            hasSectionsDictionary.Add(DrumsSectionExpert, false);
            hasSectionsDictionary.Add(DrumsSectionHard, false);
            hasSectionsDictionary.Add(DrumsSectionMedium, false);
            hasSectionsDictionary.Add(DrumsSectionEasy, false);
            
            hasSectionsDictionary.Add(CoopGuitarSectionExpert, false);
            hasSectionsDictionary.Add(CoopGuitarSectionHard, false);
            hasSectionsDictionary.Add(CoopGuitarSectionMedium, false);
            hasSectionsDictionary.Add(CoopGuitarSectionEasy, false);
            
            hasSectionsDictionary.Add(RhythmGuitarSectionExpert, false);
            hasSectionsDictionary.Add(RhythmGuitarSectionHard, false);
            hasSectionsDictionary.Add(RhythmGuitarSectionMedium, false);
            hasSectionsDictionary.Add(RhythmGuitarSectionEasy, false);
            
            hasSectionsDictionary.Add(BassSectionExpert, false);
            hasSectionsDictionary.Add(BassSectionHard, false);
            hasSectionsDictionary.Add(BassSectionMedium, false);
            hasSectionsDictionary.Add(BassSectionEasy, false);
            
            hasSectionsDictionary.Add(KeysSectionExpert, false);
            hasSectionsDictionary.Add(KeysSectionHard, false);
            hasSectionsDictionary.Add(KeysSectionMedium, false);
            hasSectionsDictionary.Add(KeysSectionEasy, false);
        }

        private void ParseSection(string[] lines, string sectionName, ParseSectionLineDelegate sectionLineDelegate)
        {
            bool isInSection = false;
            foreach (string line in lines)
            {
                if (line.Trim() == sectionName)
                {
                    Debug.Log("Found section: " + sectionName);
                    isInSection = true;
                    hasSectionsDictionary[sectionName] = true;
                }
                else if (line.Trim().StartsWith("[") && isInSection)
                {
                    // We've reached the end of the section
                    break;
                }

                if (isInSection)
                {
                    sectionLineDelegate(line);
                }
            }
        }
        
        private void ParseSongSection(string line)
        {
            if (line.StartsWith("  Name ="))
                name = line.Split('=')[1].Trim().Trim('"');
            else if (line.StartsWith("  Artist ="))
                artist = line.Split('=')[1].Trim().Trim('"');
            else if (line.StartsWith("  Charter ="))
                charter = line.Split('=')[1].Trim().Trim('"');
            else if (line.StartsWith("  Year ="))
                year = line.Split('=')[1].Trim().Trim('"').Trim(',').Trim(' ');
            else if (line.StartsWith("  Resolution ="))
            {
                string[] parts = line.Split('=');
                resolution = int.Parse(parts[1].Trim());
            }
            else if (line.StartsWith("  Offset ="))
            {
                string[] parts = line.Split('=');
                offset = float.Parse(parts[1].Trim());
            }
            else if (line.StartsWith("  MusicStream ="))
            {
                audioPath = line.Split('=')[1].Trim().Trim('"').Trim(',').Trim(' ');
                hasAudioPath = true;
            }
        }
    
        private void ParseSyncTrackSection(string line)
        {
            //  Tempo markers use the B type code, and are written like this:
            //  <Position> = B <Tempo>
            //  is a whole number representation of the desired tempo.
            //  The first 3 digits starting from the right are the decimals,
            //  giving tempos a maximum of 3 decimal places. i.e. - B 120000 is 120 BPM.
            if (line.Contains(" = B "))
            {
                string[] parts = line.Split(new[] { " = B " }, System.StringSplitOptions.RemoveEmptyEntries);
                int tick = int.Parse(parts[0].Trim());
                string[] bpmParts = parts[1].Split(' ');
                int bpm = int.Parse(bpmParts[0]);

                if (tick == 0)
                {
                    bpmStart = tick;
                }
                ticksToBpmChanges.Add(tick, bpm);
            }
        
            //  Time signature markers use the TS type code, and are written like this:
            //  <Position> = TS <Numerator> <Denominator exponent>
            //  Numerator is the numerator to use for the time signature.
            //  Denominator exponent is optional, and specifies a power of 2 to use for the denominator of the time signature.
            //  Defaults to 2 (x/4) if unspecified.
            //  This value is limited to a minimum of 0, for a resulting denominator of x/1.
            //  A time signature marker must exist at tick 0 in the chart to set the initial time signature.
            // If one is not present, a time signature of 4/4 is assumed.
            else if (line.Contains(" = TS "))
            {
                string[] parts = line.Split(new[] { " = TS " }, System.StringSplitOptions.RemoveEmptyEntries);
                int tick = int.Parse(parts[0].Trim());
                string[] timeSignatureParts = parts[1].Split(' ');
                int timeSignature = int.Parse(timeSignatureParts[0]);
                if (tick == 0)
                {
                    timeSignatureStart = timeSignature;
                }
                ticksToTimeSignatureChanges.Add(tick, timeSignature);
            }
        }
        
        private void ParseDrumsSection(string line)
        {
            if (line.Contains(" = N "))
            {
                string[] parts = line.Split(new[] { " = N " }, System.StringSplitOptions.RemoveEmptyEntries);
                float tick = float.Parse(parts[0].Trim());
                string[] noteParts = parts[1].Split(' ');
                int noteKey = int.Parse(noteParts[0]);
                float duration = float.Parse(noteParts[1]);

                Note note = new Note
                {
                    tick = tick,
                    drumNoteType = Note.GetDrumNoteType(noteKey),
                    noteDuration = duration
                };
                drumsNotes.Add(note);
            }
        }
    }
}