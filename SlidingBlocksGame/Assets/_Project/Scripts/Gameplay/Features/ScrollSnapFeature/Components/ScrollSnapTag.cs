using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    public struct ScrollSnapTag : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components", sourceClassName: "ScrollSnapTag/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : TagComponentTemplate<ScrollSnapTag>
        {
        }
    }
}