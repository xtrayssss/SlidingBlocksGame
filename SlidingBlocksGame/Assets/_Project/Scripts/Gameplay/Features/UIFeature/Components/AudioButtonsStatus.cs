using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct AudioButtonsStatus : IEcsComponent
    {
        public GameObject MusicOff;
        public GameObject MusicOn;

        public GameObject SoundOff;
        public GameObject SoundOn;

        public bool MusicIsOn;
        public bool SoundIsOn;

        private sealed class Template : ComponentTemplate<AudioButtonsStatus>
        {
        }
    }
}