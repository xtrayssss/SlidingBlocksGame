using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.VFXFeature.Components
{
    [Serializable]
    public struct DestructionVfxPrefab : IEcsComponent
    {
        public EcsEntityConnect Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.MovementFeature.Systems",
            sourceClassName: "DestructionFxPrefab/Template", sourceAssembly: "Assembly-CSharp")]
        private class Template : ComponentTemplate<DestructionVfxPrefab>
        {
        }
    }
}