using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
{
    [Serializable]
    [MetaGroup("GameProgress")]
    public struct Scores : IEcsComponent
    {
        public int Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameProgressFeature.Components",
            sourceClassName: "Scores/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<Scores>
        {
        }
    }
}