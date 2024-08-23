using System;
using DCFApixels.DragonECS;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    [Serializable]
    public struct DestructionFxPrefab : IEcsComponent
    {
        public EcsEntityConnect Value;

        private class Template : ComponentTemplate<DestructionFxPrefab>
        {
        }
    }
}