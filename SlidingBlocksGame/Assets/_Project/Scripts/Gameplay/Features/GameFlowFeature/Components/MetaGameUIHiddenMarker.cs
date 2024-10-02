using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    public struct MetaGameUIHiddenMarker : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components", sourceClassName: "MetaGameUIHiddenMarker/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : TagComponentTemplate<MetaGameUIHiddenMarker>
        {
        }
    }
}