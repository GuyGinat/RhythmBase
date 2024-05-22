using UnityEngine;

namespace Songs
{
    public static class SongExtensionMethods
    {
        public static AudioType GetUnityAudioFormat(this string format)
        {
            switch (format)
            {
                case "ogg":
                    return AudioType.OGGVORBIS;
                case "wav":
                    return AudioType.WAV;
                case "mp3":
                    return AudioType.MPEG;
                // case "opus":
                //     return AudioType.OPUS;
                default:
                    return AudioType.UNKNOWN;
            }
        }
    }
}