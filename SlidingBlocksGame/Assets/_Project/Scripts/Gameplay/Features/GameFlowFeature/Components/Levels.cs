using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    [MetaGroup("GameFlow")]
    public struct Levels : IEcsComponent
    {
        [FormerlySerializedAs("LevelIndex")] public int LevelsCount;
        public int PackIndex;

        [FormerlySerializedAs("Value")] public LevelsPack[] Pack;
        public int[][] Randoms;

        [Serializable]
        public struct LevelsPack
        {
            public ScriptableEntityTemplate[] Levels;
        }

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components",
            sourceClassName: "Levels/Template", sourceAssembly: "GameFlowFeature.Components")]
        private sealed class Template : ComponentTemplate<Levels>
        {
        }
    }
}