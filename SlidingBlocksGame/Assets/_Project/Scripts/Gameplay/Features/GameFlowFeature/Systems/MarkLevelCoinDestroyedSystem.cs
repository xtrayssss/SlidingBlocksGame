using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class MarkLevelCoinDestroyedSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CoinAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CoinTag> CoinTag;
            [Inc] public readonly EcsTagPool<DiedEvent> DiedEvent;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Exc] public readonly EcsTagPool<CoinDestroyedMarker> CoinDestroyedMarker;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out CoinAspect _))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                    levelAspect.CoinDestroyedMarker.Add(level);
            }
        }
    }
}