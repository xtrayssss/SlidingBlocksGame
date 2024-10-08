using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    public struct CloseSettingsButtonTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
            sourceClassName: "CloseSettingsButtonTag/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<CloseSettingsButtonTag>
        {
        }
    }
}