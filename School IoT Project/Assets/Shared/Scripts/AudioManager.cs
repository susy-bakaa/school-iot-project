using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;
using HQFPSTemplate;
using UnityEngine.SceneManagement;
using System.Linq;

namespace susy_baka.Shared.Audio
{
    public class AudioManager : MonoBehaviour
    {

        public static AudioManager instance;

        [BHeader("Settings")]

        public AudioMixerGroup mixerGroup;

        [BHeader("Sounds")]

        public Sound[] sounds;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();

                if (s.clip == null && !s.multipleClips && s.clips.Length > 0)
                {
                    s.multipleClips = true;
                }
                else if (s.clip != null && s.multipleClips)
                {
                    s.multipleClips = false;
                }
                else if (s.clip == null && s.clips.Length <= 0)
                {
                    Debug.LogError("No clips found for sound " + s.name);
                }

                s.source.outputAudioMixerGroup = mixerGroup;

                //if (s.sourceOverrides.Count > 1)
                //{
                //    s.sources.Capacity = s.sourceOverrides.Count;
                //    for (int i = 0; i < s.sourceOverrides.Count; i++)
                //    {
                //        s.sources[i] = s.sourceOverrides[i].gameObject.AddComponent<AudioSource>();
                //    }
                //}
                //else
                //{
                //    if (s.sourceOverrides.Count <= 0)
                //        s.source = gameObject.AddComponent<AudioSource>();
                //    else
                //        s.source = s.sourceOverrides[0].gameObject.AddComponent<AudioSource>();
                //}

                if (!s.multipleClips)
                    s.source.clip = s.clip;

                s.source.mute = s.mute;
                s.source.bypassEffects = s.bypassEffects;
                s.source.bypassListenerEffects = s.bypassListenerEffects;
                s.source.bypassReverbZones = s.bypassReverbZones;
                if (s.playOnAwake)
                {
                    if (!s.multipleClips)
                    {
                        if (s.specificSceneOnly == null)
                        {
                            s.source.playOnAwake = s.playOnAwake;
                            Play(s.name);
                        }
                        else
                        {
                            if (SceneManager.GetActiveScene().path == s.specificSceneOnly)
                            {
                                s.source.playOnAwake = s.playOnAwake;
                                Play(s.name);
                            }
                        }
                    }
                    else
                    {
                        if (s.specificSceneOnly == null)
                        {
                            s.source.playOnAwake = s.playOnAwake;
                            Play(s.name);
                        }
                        else
                        {
                            if (SceneManager.GetActiveScene().path == s.specificSceneOnly)
                            {
                                s.source.playOnAwake = s.playOnAwake;
                                Play(s.name);
                            }
                        }
                    }
                }
                s.source.loop = s.loop;

                s.source.priority = s.priority;
                s.source.panStereo = s.stereoPan;
                s.source.spatialBlend = s.spatialBlend;
                s.source.reverbZoneMix = s.reverbZoneMix;
            }
        }

        public void PlaySimple(string sound)
        {
            Play(sound);
        }

        public void Play(string sound, int index = -1)
        {
            if (sound.StartsWith("#"))
                return;

            Sound s = Array.Find(sounds, item => item.name == sound);
            if (s == null)
            {
                Debug.LogError("Sound: " + sound + " not found!");
                return;
            }

            //if (s.sourceOverrides.Count > 1)
            //{
            //    for (int i = 0; i < s.sources.Count; i++)
            //    {
            //        if (s.multipleClips && index < 0)
            //        {
            //            s.sources[i].clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
            //        }
            //        else if (index >= 0)
            //        {
            //            s.sources[i].clip = s.clips[index];
            //        }
            //        s.sources[i].volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            //        s.sources[i].pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
            //
            //        s.sources[i].Play();
            //    }
            //}
            //else
            //{
            if (s.multipleClips && index < 0)
            {
                s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
            }
            else if (index >= 0)
            {
                s.source.clip = s.clips[index];
            }
            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

            s.source.Play();
            //}
        }

        public void StopPlaying(string sound)
        {
            Sound s = Array.Find(sounds, item => item.name == sound);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + sound + " not found!");
                return;
            }
            //if (s.sourceOverrides.Count > 1)
            //{
            //    for (int i = 0; i < s.sources.Count; i++)
            //    {
            //        s.sources[i].volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            //        s.sources[i].pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
            //
            //       s.sources[i].Stop();
            //    }
            //}
            //else
            //{
            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

            s.source.Stop();
            //}
        }

        public void StopPlayingAll()
        {
            foreach (Sound s in sounds)
            {
                //if (s.sourceOverrides.Count > 1)
                //{
                //    for (int i = 0; i < s.sources.Count; i++)
                //    {
                //        s.sources[i].Stop();
                //    }
                //}
                //else
                //{
                s.source.Stop();
                //}
            }
        }
    }
}