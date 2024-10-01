using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Components
{
    [Serializable]
    public struct GameTitleConnect : IEcsComponent
    {
        public EcsEntityConnect Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components", sourceClassName: "GameTitleConnect/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<GameTitleConnect>
        {
        }
    }
}