using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class ScoresSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class LevelChangedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<LevelChangedEvent> _levelChanged;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Scores> Scores;

            [Opt] public readonly EcsTagPool<ScoresUpdatedEvent> ScoresUpdated;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out LevelChangedAspect _))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    playerAspect.Scores.Get(player).Value += 1;

                    int @event = _world.NewEntity();

                    playerAspect.ScoresUpdated.Add(@event);
                    playerAspect.TargetEntity.Add(@event).Value = player.ToEntityLong(_world);
                }
            }
        }
    }
}