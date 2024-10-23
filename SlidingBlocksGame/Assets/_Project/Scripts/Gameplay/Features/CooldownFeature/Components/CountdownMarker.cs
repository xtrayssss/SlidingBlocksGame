using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    [MetaGroup("Cooldown")]
    public struct CountdownMarker : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.CooldownFeature.Components",
            sourceClassName: "CountdownMarker/Template", sourceAssembly: "Assembly-CSharp")]
        public sealed class Template : TagComponentTemplate<CountdownMarker>
        {
        }
    }
}