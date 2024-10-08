using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
{
    [Serializable]
    [MetaGroup("GameProgress")]
    public struct CoinPrefab : IEcsComponent
    {
        public EcsEntityConnect Prefab;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.CollectionFeature.Components",
            sourceClassName: "CoinPrefab/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<CoinPrefab>
        {
        }
    }
}