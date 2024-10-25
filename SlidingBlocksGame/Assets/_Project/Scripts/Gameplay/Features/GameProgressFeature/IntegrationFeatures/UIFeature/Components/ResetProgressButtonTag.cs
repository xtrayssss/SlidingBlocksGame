using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("GameProgress/UI")]
    public struct ResetProgressButtonTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "ResetProgressButtonTag/Template", sourceAssembly: "Assembly-CSharp")]
        public class Template : TagComponentTemplate<ResetProgressButtonTag>
        {
        }
    }
}