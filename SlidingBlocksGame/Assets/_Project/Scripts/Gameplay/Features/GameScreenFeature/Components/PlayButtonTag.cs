using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameScreenFeature.Components
{
    [Serializable]
    [MetaGroup("GameScreen")]
    public struct PlayButtonTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "PlayButtonTag/Template", sourceAssembly: "UIFeature.Components")]
        private sealed class Template : TagComponentTemplate<PlayButtonTag>
        {
        }
    }
}