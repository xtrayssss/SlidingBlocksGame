using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct ParticleSystemRef : IEcsComponent
    {
        public ParticleSystem Value;

        private class Template : ComponentTemplate<ParticleSystemRef>
        {

        }
    }
}