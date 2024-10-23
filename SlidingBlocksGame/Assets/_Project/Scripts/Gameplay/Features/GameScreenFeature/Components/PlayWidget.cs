using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameScreenFeature.Components
{
    [Serializable]
    [MetaGroup("GameScreen")]
    public struct PlayWidget : IEcsComponent
    {
        public GameObject PlayButton;
        public GameObject ReplayButton;
        
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "PlayWidget/Template", sourceAssembly: "UIFeature.Components")]
        private sealed class Template : ComponentTemplate<PlayWidget>
        {
        }
    }
}