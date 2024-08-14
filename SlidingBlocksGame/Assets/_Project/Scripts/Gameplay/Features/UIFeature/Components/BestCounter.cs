using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct BestCounter : IEcsComponent
    {
        public int Value;

        private sealed class Template : ComponentTemplate<BestCounter>
        {
        }
    }
}