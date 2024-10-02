using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.Systems
{
    public class AnimalDestructionChainStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class StrategyAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ApplyDestructionStrategyRequest))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;

            [Inc] public readonly EcsTagPool<DestructionChainStrategyTag> DestructionChainTag;
        }

        private class CooldownAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;

            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
            [Opt] public readonly EcsPool<TargetEntity> Target;
        }

        private class CooldownExpiredAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(DestructionChainStrategyTag))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Opt] public readonly EcsTagPool<AnimalDestructedEvent> AnimalDestructed;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<AnimalTag> AnimalTag;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out StrategyAspect chainAspect))
            {
                if (!chainAspect.TargetEntities.Read(entity).Value.TryGetID(out int levelID))
                    continue;
                
                CooldownAspect cooldownAspect = _world.GetAspect<CooldownAspect>();

                EcsSpan animals = _world.Where(out AnimalAspect _);

                for (int index = 0; index < animals.Count; index++)
                {
                    int animal = animals[index];

                    int cooldown = _world.NewEntity();

                    cooldownAspect.Cooldowns.Add(cooldown).Duration =
                        chainAspect.Cooldowns.Read(entity).Duration * (index + 1);

                    cooldownAspect.Refresh.Add(cooldown);
                    cooldownAspect.DeleteOnExpired.Add(cooldown);
                    chainAspect.DestructionChainTag.Add(cooldown);
                    cooldownAspect.Target.Add(cooldown).Value = _world.GetEntityLong(animal);
                }

                CreateStrategyCooldown(
                    chainAspect,
                    cooldownAspect, 
                    animals,
                    target: levelID,
                    in chainAspect.Cooldowns.Read(entity));
            }

            foreach (int entity in _world.Where(out CooldownExpiredAspect cooldownExpiredAspect))
            {
                if (!cooldownExpiredAspect.Targets.Read(entity).Value.TryGetID(out int levelID))
                    continue;

                LevelAspect levelAspect = _world.GetAspect<LevelAspect>();

                if (levelAspect.IsMatches(levelID))
                    levelAspect.AnimalDestructed.Add(levelID);
            }
        }

        private void CreateStrategyCooldown(
            StrategyAspect strategyAspect,
            CooldownAspect cooldownAspect,
            EcsSpan animals,
            int target,
            in Cooldown strategyCooldown)
        {
            int cooldown = _world.NewEntity();

            strategyAspect.Cooldowns.Add(cooldown).Duration = strategyCooldown.Duration * animals.Count;
            cooldownAspect.Refresh.Add(cooldown);
            cooldownAspect.DeleteOnExpired.Add(cooldown);
            cooldownAspect.Target.Add(cooldown).Value = _world.GetEntityLong(target);
            strategyAspect.DestructionChainTag.Add(cooldown);
        }
    }
}