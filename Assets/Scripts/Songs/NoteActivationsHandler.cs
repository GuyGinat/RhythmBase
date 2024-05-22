using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Systems;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Songs
{
    public class NoteActivationsHandler : MonoBehaviour
    {
        private Dictionary<Note.DrumNoteType, SpriteRenderer> noteSpritesDictionary;
        public  Dictionary<Note.DrumNoteType, List<CustomNoteResponder>> noteRespondersDictionary;
        
        public List<CustomNoteResponder> customNoteResponders;
    
        private void Start()
        {
            noteRespondersDictionary = new Dictionary<Note.DrumNoteType, List<CustomNoteResponder>>();
            var noteTypes = Enum.GetValues(typeof(Note.DrumNoteType)).Cast<Note.DrumNoteType>();
            int currentNoteIndex = 0;

            foreach (Note.DrumNoteType drumNoteType in noteTypes)
            {
                noteRespondersDictionary.Add(drumNoteType, new List<CustomNoteResponder>());
            }

            customNoteResponders.ForEach(responder =>
            {
                if (!noteRespondersDictionary.ContainsKey(responder.currentNoteType))
                {
                    noteRespondersDictionary.Add(responder.currentNoteType, new List<CustomNoteResponder>());
                }
                noteRespondersDictionary[responder.currentNoteType].Add(responder);
            });
            
            NoteEventManager.Instance.OnNotePlayed += OnNotePlayed;
        }

        private void OnDisable()
        {
            NoteEventManager.Instance.OnNotePlayed -= OnNotePlayed;
        }

        private void OnNotePlayed(Note note)
        {
            noteRespondersDictionary[note.drumNoteType].ForEach(responder => responder.OnNotePlayed());
        }

        
    }
}