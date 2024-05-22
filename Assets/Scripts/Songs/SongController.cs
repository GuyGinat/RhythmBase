using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Songs
{
    [RequireComponent(typeof(AudioSource))]
    public class SongController : MonoBehaviour
    {
        // Get A Folder With The Song Files //
        // Get The Album Cover - image file //
        // Get Song Data - .chart file //
        // Load In Notes - object //
        // Spawn Notes In After X Ticks //

        public string songFolder = "C:/Users/guygi/Desktop/Clone Hero/Royal Blood - Out of the Black (Debugmod12)/Royal Blood - Out of the Black (Debugmod12)";

        public SongData songData;
        public Image albumCoverGUI;

        public TextMeshProUGUI songName;
        public TextMeshProUGUI artist;
        public TextMeshProUGUI charter;
        public TextMeshProUGUI year;

        public List<Note> notes;

        // Sample values for BPM and resolution
        public float currentBPM = 120.0f;  // Beats per minute
        public int resolution = 192;       // Ticks per beat

        public float bpmStartTime = 0;     // Time when the current BPM started
        public float bpmStartTick = 0;     // Tick when the current BPM started

        public float songStartTime = 0f;

        public GameObject noteObj;
        public Material[] noteMaterials;
        public Transform[] notePos;

        AudioSource audioSource;

        public float delay = 5f;

        private string audioPath;
        private bool hasAudioPath = false;
    
        public NoteActivationsHandler noteActivationsHandler;
    
        public bool useMetronome = false;
        public Metronome metronome;
        private bool isBPMSet = false;

        public void Start()
        {
            songData = SongDataLoader.LoadSongData(songFolder);
            // return;
            notes = new List<Note>();
            audioSource = GetComponent<AudioSource>();

            // Open Album Cover Image //
            // LoadChartCover();

            // Chart File - Get Song Data //
            LoadChartData();

            // Chart File - Get Note Data //
            LoadNotes();

            // Start Song //
            PlaySongAudioAsync();
            StartCoroutine(PlaySong());
        }

        IEnumerator PlaySong()
        {
            yield return null;

            // songStartTime = Time.time;  // Record the start time of the song

            foreach (var note in notes)
            {
                float noteTime = TicksToSeconds(note.tick);  // Convert tick to time
                float waitTime = noteTime - (Time.time - songStartTime) - 1f;  // Calculate how long to wait

                if (waitTime > 0)
                {
                    yield return new WaitForSeconds(waitTime);  // Wait for the right time to spawn the note
                }

                SpawnNote(note);  // Spawn the note
            }
        }

        private float time = 0;
        private void FixedUpdate()
        {
            time += Time.deltaTime;
        }

        async void PlaySongAudioAsync()
        {
            string path = Path.Combine(songFolder, hasAudioPath ? audioPath : "song.wav");
            AudioType audioType = path.Split(".")[1].GetUnityAudioFormat();
            audioSource.clip = await LoadClip(path, audioType);

            StartCoroutine(ClipPlay());
        }

        IEnumerator ClipPlay()
        {
            yield return new WaitForSeconds(0);
            if (useMetronome) metronome.StartMetronome(currentBPM, delay);
            songStartTime = Time.time;
            audioSource.Play();
        }

        [Obsolete]
        async Task<AudioClip> LoadClip(string path, AudioType audioType)
        {
            AudioClip clip = null;
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, audioType))
            {
                _ = uwr.SendWebRequest();

                // wrap tasks in try/catch, otherwise it'll fail silently
                try
                {
                    while (!uwr.isDone) await Task.Delay(5);

                    if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
                catch (Exception err)
                {
                    Debug.Log($"{err.Message}, {err.StackTrace}");
                }
            }

            return clip;
        }

        void SpawnNote(Note note)
        {
        
            NoteEventManager.Instance.NotePlayed(note);
        
        }

        void LoadChartCover()
        {
            string coverPath = Path.Combine(songFolder, "album.jpg");
            if (File.Exists(coverPath))
            {
                byte[] fileData = File.ReadAllBytes(coverPath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                Sprite albumCover = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                if (albumCoverGUI != null)
                    albumCoverGUI.sprite = albumCover;
            }
        }

        void LoadChartData()
        {
            string chartPath = Path.Combine(songFolder, "notes.chart");
            if (File.Exists(chartPath))
            {
                string[] lines = File.ReadAllLines(chartPath);
                bool isInSongSection = false;
                foreach (var line in lines)
                {
                    if (line.Trim() == "[Song]")
                    {
                        isInSongSection = true;
                    }
                    else if (line.Trim().StartsWith("[") && isInSongSection)
                    {
                        // We've reached the end of the song metadata section
                        break;
                    }

                    if (isInSongSection)
                    {
                        if (line.StartsWith("  Name ="))
                            songName.text = line.Split('=')[1].Trim().Trim('"');
                        else if (line.StartsWith("  Artist ="))
                            artist.text = line.Split('=')[1].Trim().Trim('"');
                        else if (line.StartsWith("  Charter ="))
                            charter.text = line.Split('=')[1].Trim().Trim('"');
                        else if (line.StartsWith("  Year ="))
                            year.text = line.Split('=')[1].Trim().Trim('"').Trim(',').Trim(' ');
                        else if (line.StartsWith("  MusicStream ="))
                        {
                            audioPath = line.Split('=')[1].Trim().Trim('"').Trim(',').Trim(' ');
                            hasAudioPath = true;
                        }
                    }
                }

                // Song delay timer //

                lines = File.ReadAllLines(chartPath);
                isInSongSection = false;
                foreach (var line in lines)
                {
                    if (line.Trim() == "[SyncTrack]")
                    {
                        isInSongSection = true;
                    }
                    else if (line.Trim().StartsWith("[") && isInSongSection)
                    {
                        // We've reached the end of the song metadata section
                        break;
                    }

                    if (isInSongSection)
                    {
                        if (line.StartsWith("  0 = TS "))
                        {
                            delay = line.Split("  0 = TS ")[1].Trim(' ')[0] / 14f;
                        }
                        
                    }
                }

                // Song bpm

                lines = File.ReadAllLines(chartPath);
                isInSongSection = false;
                foreach (var line in lines)
                {
                    if (line.Trim() == "[SyncTrack]")
                    {
                        isInSongSection = true;
                    }
                    else if (line.Trim().StartsWith("[") && isInSongSection)
                    {
                        // We've reached the end of the song metadata section
                        break;
                    }

                    if (isInSongSection)
                    {
                        if (line.StartsWith("  0 = B ") && !isBPMSet)
                        {
                            // isBPMSet = true;
                            string bpmLine = line.Split("  0 = B ")[1].Trim(' ');
                            // currentBPM = float.Parse(bpmLine) / 1000f;
                        }

                    }
                }
            }
        }

        void LoadNotes()
        {
            string chartPath = Path.Combine(songFolder, "notes.chart");
            if (File.Exists(chartPath))
            {
                string[] lines = File.ReadAllLines(chartPath);
                bool isNoteSection = false;
                notes = new List<Note>();

                foreach (var line in lines)
                {
                    // [ExpertDrums] - for drums
                    // [ExpertSingle] - for guitar
                    if (line.Trim() == "[ExpertDrums]") 
                    {
                        isNoteSection = true;
                        continue;
                    }
                
                    if (isNoteSection)
                    {
                        if (line.Trim().StartsWith("["))
                            break; // End of note section

                        // Process the note line
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
                            notes.Add(note);
                        
                        }
                    }
                }
            }
        }

        // Converts ticks to seconds based on the current BPM and resolution
        public float TicksToSeconds(float ticks)
        {
            float secondsPerBeat = 60 / currentBPM;
            float deltaTicks = ticks - bpmStartTick;
            float deltaBeats = deltaTicks / resolution;
            float deltaSeconds = deltaBeats * secondsPerBeat;
            return deltaSeconds + bpmStartTime;
        }

        // Converts seconds to ticks based on the current BPM and resolution
        public float SecondsToTicks(float seconds)
        {
            float secondsPerBeat = 60 / currentBPM;
            float deltaSeconds = seconds - bpmStartTime;
            float deltaBeats = deltaSeconds / secondsPerBeat;
            float deltaTicks = deltaBeats * resolution;
            return deltaTicks + bpmStartTick;
        }
    }
}