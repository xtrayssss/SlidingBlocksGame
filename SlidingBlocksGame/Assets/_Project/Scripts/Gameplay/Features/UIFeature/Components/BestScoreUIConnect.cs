using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct BestScoreUIConnect : IEcsComponent
    {
        public EcsEntityConnect Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "BestScoreUIConnect/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<BestScoreUIConnect>
        {
        }
    }
}