using System;
using _Project.Scripts.Gameplay.Templates;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    [MetaGroup("GameField")]
    public struct GameFieldAlgorithms : IEcsComponent
    {
        public EntityTemplate[] Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFieldFeature.Components",
            sourceClassName: "GameFieldAlgorithms/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<GameFieldAlgorithms>
        {
        }
    }
}