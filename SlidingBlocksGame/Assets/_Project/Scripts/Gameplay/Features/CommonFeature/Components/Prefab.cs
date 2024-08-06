using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct Prefab : IEcsComponent
    {
        public EcsEntityConnect Value;
        
        private sealed class Template : ComponentTemplate<Prefab>
        {
        }
    }
}