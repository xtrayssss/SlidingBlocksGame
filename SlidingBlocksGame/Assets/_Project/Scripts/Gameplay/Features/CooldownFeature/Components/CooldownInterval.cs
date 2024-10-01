using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    public struct CooldownInterval : IEcsComponent
    {
        public float Elapsed;
        public float Interval;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CooldownFeature.Components", sourceClassName: "CooldownInterval/Template", sourceAssembly: "Assembly-CSharp")]
		private sealed class Template : ComponentTemplate<CooldownInterval>
        {
        }
    }
}