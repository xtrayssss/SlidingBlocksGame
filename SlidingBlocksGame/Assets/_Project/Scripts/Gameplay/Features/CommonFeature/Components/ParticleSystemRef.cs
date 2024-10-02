using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct ParticleSystemRef : IEcsComponent
    {
        public ParticleSystem Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "ParticleSystemRef/Template", sourceAssembly: "Assembly-CSharp")]


		private class Template : ComponentTemplate<ParticleSystemRef>
        {
        }
    }
}