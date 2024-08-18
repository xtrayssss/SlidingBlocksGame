using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct PhysicView : IEcsComponent
    {
        public GameObject Value;

        private sealed class Template : ComponentTemplate<PhysicView>
        {
        }
    }
}