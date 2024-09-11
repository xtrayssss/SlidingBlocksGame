using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CollectFeature.Components
{
    [Serializable]
    public struct CollectedTargetEntity : IEcsComponent
    {
        public entlong Value;
    }
}