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
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyViewRequest;
            [Opt] public readonly EcsTagPool<DeathEvent> DeathEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out CooldownExpiredAspect cooldownExpiredAspect))
            {
                DestructibleAspect destructibleAspect = _world.GetAspect<DestructibleAspect>();

                if (cooldownExpiredAspect.Targets.Read(entity).Value.TryGetID(out int destructibleID) &&
                    destructibleAspect.IsMatches(destructibleID))
                {
                    if (destructibleAspect.GoConnects.Has(destructibleID))
                        destructibleAspect.DestroyViewRequest.TryAdd(destructibleID);

                    destructibleAspect.DeathEvent.Add(destructibleID);
                }
            }
        }
    }
}