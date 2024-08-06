using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct AnimalTag : IEcsComponent
    {
        public sealed class Wrapper : ComponentTemplate<AnimalTag>
        {
        }
    }
}