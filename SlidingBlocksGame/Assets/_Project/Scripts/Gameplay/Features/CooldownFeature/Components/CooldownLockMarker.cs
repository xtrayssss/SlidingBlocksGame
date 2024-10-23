using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Components
{
    [Serializable]
    [MetaGroup("Cooldown")]
    public struct CooldownLockMarker : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CooldownFeature.Components", sourceClassName: "CooldownLockMarker/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : TagComponentTemplate<CooldownLockMarker>
        {
        }
    }
}