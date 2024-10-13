using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    public struct PlayerCfgRef : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.PlayerFeature.Components",
            sourceClassName: "PlayerCfgRef/Template", sourceAssembly: "PlayerFeature.Components")]
        private sealed class Template : ComponentTemplate<PlayerCfgRef>
        {
        }
    }
}