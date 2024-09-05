using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct CoinsProgressionCurve : IEcsComponent
    {
        public AnimationCurve Value;

        private sealed class Template : ComponentTemplate<CoinsProgressionCurve>
        {
        }
    }
}