using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    public struct GameScreenPrefab : IEcsComponent
    {
        public EcsEntityConnect Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components", sourceClassName: "GameScreenPrefab/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<GameScreenPrefab>
        {
        }
    }
}