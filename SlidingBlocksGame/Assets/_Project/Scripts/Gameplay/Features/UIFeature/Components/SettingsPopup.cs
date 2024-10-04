using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct SettingsPopup : IEcsComponent
    {
        public Tween OpenCloseTween;

        public GameObject SoundOff;
        public GameObject SoundOn;
        public GameObject MusicOff;
        public GameObject MusicOn;

        private sealed class Template : ComponentTemplate<SettingsPopup>
        {
        }
    }
}