using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct Purchase : IEcsComponent
    {
        public int Price;
        public uint ProductIndex;

        private sealed class Template : ComponentTemplate<Purchase>
        {
        }
    }
}