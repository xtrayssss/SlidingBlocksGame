using System;
using DCFApixels.DragonECS;
using UnityEngine.Serialization;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
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

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components", sourceClassName: "Levels/LevelsPack/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<Levels>
        {
        }
    }
}