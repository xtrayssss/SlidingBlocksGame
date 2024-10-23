using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    [MetaGroup("GameFlow")]
    public struct LevelLifeTimeMarker : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components",
            sourceClassName: "LevelLifeTimeMarker/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : TagComponentTemplate<LevelLifeTimeMarker>
        {
        }
    }
}