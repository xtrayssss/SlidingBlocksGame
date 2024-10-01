using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct AnimalTag : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "AnimalTag/Wrapper", sourceAssembly: "Assembly-CSharp")]
		public sealed class Wrapper : TagComponentTemplate<AnimalTag>
        {
        }
    }
}