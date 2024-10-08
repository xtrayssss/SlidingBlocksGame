using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
{
    [Serializable]
    [MetaGroup("GameProgress")]
    public struct CoinTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.CollectFeature.Components",
            sourceClassName: "CoinTag/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<CoinTag>
        {
        }
    }
}