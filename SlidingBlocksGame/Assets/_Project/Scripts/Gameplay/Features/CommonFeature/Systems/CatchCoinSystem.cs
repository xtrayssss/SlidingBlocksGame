using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using DCFApixels.DragonECS;

<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/Systems/CatchCoinSystem.cs
namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
========
namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameProgressFeature/Systems/CatchCoinSystem.cs
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
        }
    }
}