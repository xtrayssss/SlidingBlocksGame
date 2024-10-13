using System;
using System.Collections;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct OpenCloseSequence : IEcsComponent
    {
        public Sequence Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components", sourceClassName: "OpenCloseSequence/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<OpenCloseSequence>
        {
        }
    }
}