using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CollectFeature.Components
{
    [Serializable]
    public struct Coins : IEcsComponent
    {
        public int Value;

        private sealed class Template : ComponentTemplate<Coins>
        {
        }
    }
}