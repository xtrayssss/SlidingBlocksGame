using System.Collections.Generic;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    public struct Units : IEcsComponent
    {
        public EcsGroup Value;
        public IEnumerable<entlong> Filtered;
    }
}