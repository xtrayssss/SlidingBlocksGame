using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Systems
{
    public class CatchGameFieldEventsSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class GameFieldGeneratedRequestAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CatchGameFieldGeneratedRequest))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;

            [Opt] public readonly EcsTagPool<GameFieldGeneratedEvent> GameFieldGeneratedEvent;
        }

        private class GameFieldDestructedRequestAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldDestructedRequest))]
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;

            [Opt] public readonly EcsTagPool<GameFieldDestructedEvent> GameFieldDestructedEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GameFieldGeneratedRequestAspect aspect))
            {
                if (aspect.TargetEntities.Read(entity).Value.TryGetID(out int targetID))
                    aspect.GameFieldGeneratedEvent.Add(targetID);

                _world.DelEntity(entity);
            }

            foreach (int entity in _world.Where(out GameFieldDestructedRequestAspect aspect))
            {
                if (aspect.TargetEntities.Read(entity).Value.TryGetID(out int targetID))
                    aspect.GameFieldDestructedEvent.Add(targetID);

                _world.DelEntity(entity);
            }
        }
    }
}