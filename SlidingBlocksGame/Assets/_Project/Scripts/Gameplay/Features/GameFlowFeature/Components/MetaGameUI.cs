using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    public struct MetaGameUI : IEcsComponent
    {
        public GameObject[] Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components", sourceClassName: "MetaGameUI/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<MetaGameUI>
        {
        }
    }
}