using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    [MetaGroup("Movement")]
    public struct MovementStrategyCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.MovementFeature.Components",
            sourceClassName: "MovementStrategyCfg/Template", sourceAssembly: "MovementFeature.Components")]
        private sealed class Template : ComponentTemplate<MovementStrategyCfg>
        {
        }
    }
}