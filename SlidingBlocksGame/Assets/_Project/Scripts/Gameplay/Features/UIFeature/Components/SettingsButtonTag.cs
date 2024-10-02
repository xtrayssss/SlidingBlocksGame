using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct SettingsButtonTag : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "SettingsButtonTag/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : TagComponentTemplate<SettingsButtonTag>
        {
        }
    }
}