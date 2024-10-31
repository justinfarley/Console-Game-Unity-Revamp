using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedLobsterStudios.Util.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        #region Fields

        [SerializeField]
        private List<Sound> sounds;
        private SoundType filterType;

        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();

            foreach(Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.priority = 0;
                s.source.clip = s.clip;
                s.source.loop = s.loop;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
            }
        }
        private void OnValidate()
        {
            if (sounds == null) return;

            for(int i = 0; i < sounds.Count; i++)
                sounds[i].identifier = i;
        }

        #endregion

        #region Public API

        /// <summary>
        /// "Any" = no filter <para></para>
        /// Make sure to set filter back after done or undesirable results may occur
        /// </summary> 2413
        public List<Sound> SetFilter(SoundType filter)
        {
            filterType = filter;
            return sounds;
        }

        /// <returns>the newly playing sources if they exist</returns>
        public List<AudioSource> Play(params int[] ids)
        {
            List<AudioSource> sources = sounds.Filter(filterType).FindAll(s => ids.Any(x => x == s.identifier)).Select(x => x.source).ToList();
            sources.ForEach(x => x.Play());
            return sources;
        }

        /// <returns>the newly playing sources if they exist</returns>
        public List<AudioSource> Play(int[] ids, float pitch = 1f)
        {
            List<AudioSource> sources = sounds.Filter(filterType).FindAll(s => ids.Any(x => x == s.identifier)).Select(x => x.source).ToList();
            sources.ForEach(x =>
            {
                x.Play();
                x.pitch = pitch;
            });
            return sources;
        }

        public void Stop(params int[] ids)
        {
            List<AudioSource> sources = sounds.Filter(filterType).FindAll(s => ids.Any(x => x == s.identifier)).Select(x => x.source).ToList();
            sources.ForEach(s => s.Stop());
        }

        public void Mute(params int[] ids)
        {
            List<AudioSource> sources = sounds.Filter(filterType).FindAll(s => ids.Any(x => x == s.identifier)).Select(x => x.source).ToList();
            sources.ForEach(s => s.pitch = 0f);
        }

        /// <summary>
        /// Applies function to all specified ids
        /// </summary>
        public void ForEach(int[] ids, Action<AudioSource> func = null)
        {
            List<AudioSource> sources = sounds.Filter(filterType).FindAll(s => ids.Any(x => x == s.identifier)).Select(x => x.source).ToList();
            sources.ForEach(x => func?.Invoke(x));
        }

        public void StopAll() => sounds.Filter(filterType).Select(x => x.source).Where(x => x.isPlaying).ToList().ForEach(x => x.Stop());

        public List<AudioSource> FindPlaying() => sounds.Filter(filterType).Select(x => x.source).Where(x => x.isPlaying).ToList();

        public void FadeAudio(int id, float time, float startVol = 0f, float endVol = 1f) => StartCoroutine(FadeAudio_cr(id, time, startVol, endVol));

        #endregion

        #region Private API



        private IEnumerator FadeAudio_cr(int id, float time, float startVol, float endVol)
        {
            AudioSource source = sounds.ToList().Find(x => x.identifier == id).source;
            float start = startVol;
            float end = endVol;
            source.volume = startVol;

            if (!source.isPlaying)
                source.Play();
            
            for(float f = 0; f < time; f += Time.deltaTime)
            {
                float elapsed = f / time;
                source.volume = Mathf.Lerp(startVol, endVol, elapsed);
                yield return null;
            }
            source.volume = endVol;
        }

        #endregion
    }
    public static class AudioExtensions
    {
        //extension for filtering
        public static List<Sound> Filter(this List<Sound> source, SoundType filterType)
        {
            if (filterType == SoundType.Any) return source;

            return source.Where(x => x.type == filterType).ToList();
        }
    }
}