using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct BlockTag : IEcsComponent
    {
        [Serializable]
        public sealed class Wrapper : ComponentTemplate<BlockTag>
        {
        }
    }
}