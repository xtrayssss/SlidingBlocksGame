using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Components
{
    [Serializable]
    [MetaGroup("Animal")]
    public struct DestructionAnimalStrategyCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.AnimalFeature.Components",
            sourceClassName: "DestructionAnimalStrategyCfg/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<DestructionAnimalStrategyCfg>
        {
        }
    }
}