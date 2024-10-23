using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    [MetaGroup("Cooldown")]
    public struct Cooldown : IEcsComponent
    {
        public float Elapsed;
        public float Duration;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CooldownFeature.Components", sourceClassName: "Cooldown/Template", sourceAssembly: "Assembly-CSharp")]


		public sealed class Template : ComponentTemplate<Cooldown>
        {
        }
    }
}