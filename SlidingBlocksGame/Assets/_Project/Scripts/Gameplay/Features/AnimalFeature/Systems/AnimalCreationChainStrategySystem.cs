using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.Systems
{
    public class AnimalCreationChainStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class StrategyAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ApplyCreationStrategyRequest))]
            [Inc] public readonly EcsTagPool<CreationChainStrategyTag> CreationChainStrategyTag;

            [Inc] public readonly EcsPool<Cooldown> Cooldowns;

            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
            [Opt] public readonly EcsPool<TargetEntity> Target;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Opt] public readonly EcsTagPool<AnimalPositionedEvent> AnimalPositionedEvent;
            [Opt] public readonly EcsTagPool<AnimalPositionedMarker> AnimalPositionedMarker;
        }

        private class CooldownExpiredAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(CreationChainStrategyTag))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;

            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Opt] public readonly EcsTagPool<AnimalSpawnedEvent> AnimalSpawnedEvent;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LevelAspect _))
            {
                foreach (int strategy in _world.Where(out StrategyAspect strategyAspect))
                {
                    CooldownExpiredAspect cooldownExpiredAspect = _world.GetAspect<CooldownExpiredAspect>();

                    EcsSpan animals = _world.Where(out AnimalAspect _);

                    ref readonly Cooldown strategyCooldown = ref strategyAspect.Cooldowns.Read(strategy);

                    for (int index = 0; index < animals.Count; index++)
                    {
                        int cooldown = _world.NewEntity();

                        cooldownExpiredAspect.Cooldowns.Add(cooldown).Duration =
                            strategyCooldown.Duration * (index + 1);

                        cooldownExpiredAspect.Refresh.Add(cooldown);

                        strategyAspect.CreationChainStrategyTag.Add(cooldown);

                        cooldownExpiredAspect.DeleteOnExpired.Add(cooldown);
                        cooldownExpiredAspect.Targets.Add(cooldown).Value = _world.GetEntityLong(animals[index]);
                    }

                    CreateStrategyCooldown(strategyAspect, cooldownExpiredAspect, animals, level, in strategyCooldown);
                }
            }

            foreach (int entity in _world.Where(out CooldownExpiredAspect cooldownAspect))
            {
                if (!cooldownAspect.Targets.Read(entity).Value.TryGetID(out int levelID))
                    continue;

                LevelAspect levelAspect = _world.GetAspect<LevelAspect>();

                if (levelAspect.IsMatches(levelID))
                {
                    levelAspect.AnimalPositionedEvent.Add(levelID);
                    levelAspect.AnimalPositionedMarker.Add(levelID);
                }
            }

            foreach (int entity in _world.Where(out CooldownExpiredAspect cooldownAspect))
            {
                if (!cooldownAspect.Targets.Read(entity).Value.TryGetID(out int animalID))
                    continue;

                AnimalAspect levelAspect = _world.GetAspect<AnimalAspect>();

                if (levelAspect.IsMatches(animalID)) 
                    levelAspect.AnimalSpawnedEvent.Add(animalID);
            }
        }

        private void CreateStrategyCooldown(
            StrategyAspect strategyAspect,
            CooldownExpiredAspect cooldownExpiredAspect,
            EcsSpan animals,
            int level,
            in Cooldown strategyCooldown)
        {
            int cooldown = _world.NewEntity();

            strategyAspect.Cooldowns.Add(cooldown).Duration = strategyCooldown.Duration * animals.Count;
            cooldownExpiredAspect.Refresh.Add(cooldown);
            strategyAspect.DeleteOnExpired.Add(cooldown);
            strategyAspect.Target.Add(cooldown).Value = _world.GetEntityLong(level);
            strategyAspect.CreationChainStrategyTag.Add(cooldown);
        }
    }
}