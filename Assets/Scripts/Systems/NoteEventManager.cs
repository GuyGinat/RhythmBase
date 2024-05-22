using System;
using Songs;
using UnityEngine;
using Utils;

namespace Systems
{
    [DefaultExecutionOrder(-100)]
    public class NoteEventManager : MonoBehaviourSingleton<NoteEventManager>
    {
        public Action<Note> OnNotePlayed;
        public void NotePlayed(Note note)
        {
            OnNotePlayed?.Invoke(note);
        }
    }
}