using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    public struct FadeAlphaEffectFactor : IEcsComponent
    {
        public float Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "FadeAlphaEffectFactor/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<FadeAlphaEffectFactor>
        {
        }
    }
}