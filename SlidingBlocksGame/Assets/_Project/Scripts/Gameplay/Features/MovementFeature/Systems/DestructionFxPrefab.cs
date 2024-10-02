using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    [Serializable]
    public struct DestructionFxPrefab : IEcsComponent
    {
        public EcsEntityConnect Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.MovementFeature.Systems", sourceClassName: "DestructionFxPrefab/Template", sourceAssembly: "Assembly-CSharp")]


		private class Template : ComponentTemplate<DestructionFxPrefab>
        {
        }
    }
}