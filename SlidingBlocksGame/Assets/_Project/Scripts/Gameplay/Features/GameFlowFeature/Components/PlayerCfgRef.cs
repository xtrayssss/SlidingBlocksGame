using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    public struct PlayerCfgRef : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components", sourceClassName: "PlayerCfgRef/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<PlayerCfgRef>
        {
        }
    }
}