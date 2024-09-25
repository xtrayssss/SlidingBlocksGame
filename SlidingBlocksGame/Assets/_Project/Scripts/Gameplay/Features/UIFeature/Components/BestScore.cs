using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct BestScore : IEcsComponent
    {
        public int Value;

        private sealed class Template : ComponentTemplate<BestScore>
        {
        }
    }
}