using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.IntegrationFeatures.DestructionFeature.Systems
{
    public class CoinDestructionSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class CoinAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [ExcImplicit(typeof(DiedEvent))]
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyView;

            [Opt] public readonly EcsTagPool<CoinCollectAnimationCompletedEvent> CoinCollectAnimationCompletedEvent;
            [Opt] public readonly EcsTagPool<CoinDestroyAnimationCompletedEvent> CoinDestroyAnimationCompletedEvent;
        }

        private class ViewDestroyedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [IncImplicit(typeof(ViewDestroyedEvent))]
            [Exc] public readonly EcsTagPool<DiedEvent> DiedEvent;
        }

        public void Run()
        {
            foreach (int coin in _world.Where(out CoinAspect coinAspect))
            {
                if (coinAspect.CoinCollectAnimationCompletedEvent.Has(coin) ||
                    coinAspect.CoinDestroyAnimationCompletedEvent.Has(coin))
                {
                    coinAspect.DestroyView.Add(coin);
                }
            }

            foreach (int coin in _world.Where(out ViewDestroyedAspect coinAspect))
                coinAspect.DiedEvent.Add(coin);
        }
    }
}