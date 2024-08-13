using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct LevelIndex : IEcsComponent, IEcsComponentLifecycle<LevelIndex>
    {
        public int Value;
        public int Pack;
        public void Enable(ref LevelIndex component) => 
            component.Value = -1;

        public void Disable(ref LevelIndex component) => 
            component.Value = -1;
    }
}