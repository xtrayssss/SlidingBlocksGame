using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct HUDPrefab : IEcsComponent
    {
        public EcsEntityConnect Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components", sourceClassName: "HUDPrefab/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<HUDPrefab>
        {
        }
    }
}