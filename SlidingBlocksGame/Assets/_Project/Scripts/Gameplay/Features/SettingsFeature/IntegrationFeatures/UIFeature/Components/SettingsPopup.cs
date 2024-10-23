using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.SettingsFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Settings/UI")]
    public struct SettingsPopup : IEcsComponent
    {
        public Tween OpenCloseTween;

        public GameObject SoundOff;
        public GameObject SoundOn;
        public GameObject MusicOff;
        public GameObject MusicOn;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "SettingsPopup/Template", sourceAssembly: "UIFeature.Components")]
        private sealed class Template : ComponentTemplate<SettingsPopup>
        {
        }
    }
}