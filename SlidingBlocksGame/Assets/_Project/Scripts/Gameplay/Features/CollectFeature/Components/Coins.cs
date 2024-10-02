using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CollectFeature.Components
{
    [Serializable]
    public struct Coins : IEcsComponent
    {
        public int Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CollectFeature.Components", sourceClassName: "Coins/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<Coins>
        {
        }
    }
}