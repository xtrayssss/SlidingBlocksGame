using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct CreationAnimalStrategyCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components", sourceClassName: "CreationAnimalStrategyCfg/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<CreationAnimalStrategyCfg>
        {
        }
    }
}