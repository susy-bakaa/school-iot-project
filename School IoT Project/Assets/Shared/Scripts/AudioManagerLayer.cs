using UnityEngine;

namespace susy_baka.Shared.Audio
{
    public class AudioManagerLayer : MonoBehaviour
    {
        private AudioManager master;

        void Awake()
        {
            if (FindObjectOfType<AudioManager>() != null)
                master = FindObjectOfType<AudioManager>();
        }

        public void Play(string sound)
        {
            master.Play(sound);
        }

        public void StopPlaying(string sound)
        {
            master.StopPlaying(sound);
        }

        public void StopPlayingAll()
        {
            master.StopPlayingAll();
        }
    }
}