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
            sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "CloseTutorialButtonTag/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<CloseTutorialButtonTag>
        {
        }
    }
}