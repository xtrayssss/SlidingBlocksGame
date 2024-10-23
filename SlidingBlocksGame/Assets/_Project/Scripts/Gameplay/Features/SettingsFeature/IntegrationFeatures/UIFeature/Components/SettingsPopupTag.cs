using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.SettingsFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("Settings/UI")]
    public struct SettingsPopupTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "SettingsPopupTag/Template", sourceAssembly: "UIFeature.Components")]
        private sealed class Template : TagComponentTemplate<SettingsPopupTag>
        {
        }
    }
}