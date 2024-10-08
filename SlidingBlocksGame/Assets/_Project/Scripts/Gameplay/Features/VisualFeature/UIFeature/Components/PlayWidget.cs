using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct PlayWidget : IEcsComponent
    {
        public GameObject PlayButton;
        public GameObject ReplayButton;
        
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "PlayWidget/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<PlayWidget>
        {
        }
    }
}