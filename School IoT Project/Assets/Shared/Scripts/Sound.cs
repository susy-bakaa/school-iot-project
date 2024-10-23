using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using HQFPSTemplate;

namespace susy_baka.Shared.Audio
{
    [System.Serializable]
    public class Sound
    {
        [BHeader("Base")]
        public string name;
        public bool multipleClips = false;
        [HideIf("multipleClips")]
        [AllowNesting]
        public AudioClip clip;
        [NaughtyAttributes.ShowIf("multipleClips")]
        [AllowNesting]
        public NestedArray<AudioClip> clips;
        public AudioMixerGroup mixerGroup;

        [BHeader("Flags")]
        public bool mute;
        public bool bypassEffects = false;
        public bool bypassListenerEffects = false;
        public bool bypassReverbZones = false;
        public bool playOnAwake = false;
        [NaughtyAttributes.ShowIf("playOnAwake")]
        [AllowNesting]
        [Mirror.Scene] public string specificSceneOnly;
        public bool loop = false;

        [BHeader("Sliders")]
        [Range(0, 256)]
        public int priority = 128;

        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0f, 1f)]
        public float volumeVariance = 0f;

        [Range(-3f, 3f)]
        public float pitch = 1f;
        [Range(0f, 1f)]
        public float pitchVariance = 0f;

        [Range(-1f,1f)]
        public float stereoPan = 0f;
        [Range(0f, 1f)]
        public float spatialBlend = 0f;
        [Range(0f, 1.1f)]
        public float reverbZoneMix = 1f;

        [HideInInspector]
        public AudioSource source;
        [HideInInspector]
        public List<AudioSource> sources;

        //[Tooltip("Custom GameObjects which this sound will be attached to during runtime.")]
        //public List<Transform> sourceOverrides = null;
    }
}

[System.Serializable]
public class NestedArray<T>
{
    public T[] array;

    public NestedArray(T[] initialArray)
    {
        array = initialArray;
    }

    public NestedArray(int length)
    {
        array = new T[length];
    }

    public T[] Array
    {
        get { return array; }
        set { array = value; }
    }

    public int Length
    {
        get { return array.Length; }
    }

    public T this[int index]
    {
        get { return array[index]; }
        set { array[index] = value; }
    }

    public static implicit operator T[](NestedArray<T> nestedArray)
    {
        return nestedArray.array;
    }
}