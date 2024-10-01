using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Components
{
    [Serializable]
    public struct SideAnimals : IEcsComponent
    {
        public EcsGroup Value;
    }
}