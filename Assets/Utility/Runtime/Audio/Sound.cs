using UnityEngine;

namespace RedLobsterStudios.Util.Audio
{
    public enum SoundType
    {
        Any,
        Music,
        Effect,
    }

    [System.Serializable]
    public class Sound
    {
        public int identifier;
        public AudioClip clip;

        public SoundType type;

        [Range(0f, 1f)]
        public float volume;
        [Range(-1f, 3f)]
        public float pitch;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
}

