using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.Systems
{
    public class MovementAnimalsChainStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class StrategyAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ApplyMovementStrategyRequest))]
            [Inc] public readonly EcsTagPool<ChainMovementStrategyTag> ChainMovementStrategyTag;

            [Inc] public readonly EcsPool<TargetEntities> TargetEntities;

            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
        }

        private class CooldownAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<Cooldown> Cooldowns;
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out StrategyAspect chainAspect))
            {
                ref TargetEntities animals = ref chainAspect.TargetEntities.Get(entity);

                for (int index = 0; index < animals.Value.Count; index++)
                {
                    int movable = animals.Value[index];

                    int cooldown = _world.NewEntity();

                    CooldownAspect cooldownAspect = _world.GetAspect<CooldownAspect>();

                    cooldownAspect.Cooldowns.Add(cooldown).Duration =
                        chainAspect.Cooldowns.Read(entity).Duration * index;
                    cooldownAspect.Refresh.Add(cooldown);

                    chainAspect.ChainMovementStrategyTag.Add(cooldown);

                    cooldownAspect.DeleteOnExpired.Add(cooldown);
                    cooldownAspect.TargetEntity.Add(cooldown).Value = movable.ToEntityLong(_world);
                }
            }
        }
    }
}