using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class GameAudio : MonoBehaviour
    {
        [Serializable]
        public struct SfxSources
        {
            public AudioSource Sfx;
            public AudioSource Normal;
            public AudioSource Special;
        }

        public static GameAudio Instance { get; private set; }

        public SfxSources SfxSource;
        public AudioSource MusicSource;

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