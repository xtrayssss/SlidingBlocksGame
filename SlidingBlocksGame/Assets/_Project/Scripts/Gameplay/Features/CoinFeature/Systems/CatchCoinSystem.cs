using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Systems
{
    public class CatchCoinSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        public void Run()
        {
            foreach (int entity in _world.Where(out CoinCatcherAspect.CoinSpawnedCatcher catcherAspect))
            {
                CommonCatcherAspect commonCatcherAspect = catcherAspect.CommonCatcherAspect;

                ref readonly TargetEntity targetEntity =
                    ref commonCatcherAspect.TargetEntities.Read(entity);

                if (targetEntity.Value.TryGetID(out int targetID))
                    catcherAspect.CoinSpawnedEvent.Add(targetID);

                _world.DelEntity(entity);
            }

            foreach (int entity in _world.Where(
                         out CoinCatcherAspect.CoinCollectAnimationCompletedCatcher catcherAspect))
            {
                CommonCatcherAspect commonCatcherAspect = catcherAspect.CommonCatcherAspect;

                ref readonly TargetEntity targetEntity =
                    ref commonCatcherAspect.TargetEntities.Read(entity);

                if (targetEntity.Value.TryGetID(out int targetID))
                    catcherAspect.CoinCollectAnimationCompletedEvent.Add(targetID);

                _world.DelEntity(entity);
            }

            foreach (int entity in _world.Where(
                         out CoinCatcherAspect.CoinDestroyAnimationCompletedCatcher catcherAspect))
            {
                CommonCatcherAspect commonCatcherAspect = catcherAspect.CommonCatcherAspect;

                ref readonly TargetEntity targetEntity =
                    ref commonCatcherAspect.TargetEntities.Read(entity);

                if (targetEntity.Value.TryGetID(out int targetID))
                    catcherAspect.CoinDestroyAnimationCompletedEvent.Add(targetID);

                _world.DelEntity(entity);
            }
        }
    }
}