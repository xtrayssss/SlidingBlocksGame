using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct TargetEntities : IEcsComponent
    {
        public EcsGroup Value;
    }
}