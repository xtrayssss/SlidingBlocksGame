using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestructionFeature.Systems
{
    public class DestructionChainStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CooldownExpiredAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(DestructionChainStrategyTag))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class DestructibleAspect : EcsAspectAuto
        {
            [Exc] public readonly EcsTagPool<DestructibleStrategyCompletedEvent> DestructibleStrategyCompletedEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out CooldownExpiredAspect cooldownExpiredAspect))
            {
                DestructibleAspect destructibleAspect = _world.GetAspect<DestructibleAspect>();

                if (cooldownExpiredAspect.Targets.Read(entity).Value.TryGetID(out int destructibleID))
                    destructibleAspect.DestructibleStrategyCompletedEvent.Add(destructibleID);
            }
        }
    }
}