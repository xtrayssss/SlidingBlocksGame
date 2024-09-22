using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class CatchMovementTweenSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        public void Run()
        {
            foreach (int entity in _world.Where(out MovementTweenCatcherAspect catcherAspect))
            {
                ref readonly TargetEntity targetEntity =
                    ref catcherAspect.CommonCatcherAspect.TargetEntities.Read(entity);

                if (targetEntity.Value.TryGetID(out _))
                {
                    int callback = _world.NewEntity();
                    _world.GetPool<MovementTweenCompletedEvent>().Add(callback);
                    _world.GetPool<TargetEntity>().Add(callback).Value = targetEntity.Value;
                }

                _world.DelEntity(entity);
            }
        }
    }
}