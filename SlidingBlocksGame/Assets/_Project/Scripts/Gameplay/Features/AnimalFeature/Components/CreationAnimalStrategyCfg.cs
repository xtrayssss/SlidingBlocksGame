using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.Components
{
    [Serializable]
    [MetaGroup("Animal")]
    public struct CreationAnimalStrategyCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.GameFlowFeature.Components",
            sourceClassName: "CreationAnimalStrategyCfg/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<CreationAnimalStrategyCfg>
        {
        }
    }
}