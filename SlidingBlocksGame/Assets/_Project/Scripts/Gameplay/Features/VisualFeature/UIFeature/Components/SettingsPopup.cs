using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
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

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "SettingsPopup/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<SettingsPopup>
        {
        }
    }
}