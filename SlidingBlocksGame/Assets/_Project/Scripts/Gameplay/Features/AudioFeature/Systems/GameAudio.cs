using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class GameAudio : MonoBehaviour
    {
        [Serializable]
        public struct SfxAudio
        {
            public AudioMixerGroup Mixer;
            [FormerlySerializedAs("Sfx")] public AudioSource Base;
            public AudioSource Normal;
            public AudioSource Special;
        }

        [Serializable]
        public struct MusicAudio
        {
            public AudioSource Source;
            public AudioMixerGroup Mixer;
        }

        public SfxAudio Sfx;

        public MusicAudio Music;

        public static GameAudio Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}