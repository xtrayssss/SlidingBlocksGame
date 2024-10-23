using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.TutorialFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Tutorial/UI")]
    public struct CloseTutorialButtonTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "CloseTutorialButtonTag/Template", sourceAssembly: "UIFeature.Components")]
        private sealed class Template : TagComponentTemplate<CloseTutorialButtonTag>
        {
        }
    }
}