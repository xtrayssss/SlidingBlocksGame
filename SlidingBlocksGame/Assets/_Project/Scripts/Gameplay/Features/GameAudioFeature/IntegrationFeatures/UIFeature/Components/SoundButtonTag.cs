using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameAudioFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("GameAudio/UI")]
    public struct SoundButtonTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "SoundButtonTag/Template", sourceAssembly: "UIFeature.Components")]
        private sealed class Template : TagComponentTemplate<SoundButtonTag>
        {
        }
    }
}