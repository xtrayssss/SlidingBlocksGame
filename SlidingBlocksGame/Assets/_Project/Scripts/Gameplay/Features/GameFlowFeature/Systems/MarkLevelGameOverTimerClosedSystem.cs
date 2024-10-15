using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class MarkLevelGameOverTimerClosedSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Exc] public readonly EcsTagPool<GameOverTimerClosedMarker> GameOverTimerClosedMarker;

            [Opt] public readonly EcsTagPool<LevelVictoryMarker> LevelVictoryMarker;
            [Opt] public readonly EcsTagPool<LevelDefeatMarker> LevelDefeatMarker;
        }

        private class GameOverTimerClosedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameOverTimerTag> GameOverTimerTag;
            [Inc] public readonly EcsTagPool<GameOverTimerClosedEvent> GameOverTimerClosedEvent;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out GameOverTimerClosedAspect _))
            {
                foreach (int level in _world.Where(out LevelAspect levelAspect))
                {
                    if (levelAspect.LevelVictoryMarker.Has(level) || levelAspect.LevelDefeatMarker.Has(level))
                        levelAspect.GameOverTimerClosedMarker.Add(level);
                }
            }
        }
    }
}