using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameScreenFeature.Components
{
    [Serializable]
    [MetaGroup("GameScreen")]
    public struct GameScreenPrefab : IEcsComponent
    {
        public EcsEntityConnect Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components",
            sourceClassName: "GameScreenPrefab/Template", sourceAssembly: "GameFlowFeature.Components")]
        private sealed class Template : ComponentTemplate<GameScreenPrefab>
        {
        }
    }
}