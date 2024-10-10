using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct GameFieldAlgorithmCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFieldFeature.Components",
            sourceClassName: "GameFieldAlgorithmCfg/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<GameFieldAlgorithmCfg>
        {
        }
    }
}