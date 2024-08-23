using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class AnimalCreationChainStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ChainAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreationChainTag))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;

            [Opt] public readonly EcsTagPool<CreationChainTag> CreationChain;
            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
            [Opt] public readonly EcsPool<TargetEntity> Target;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateAnimalsRequest))]
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        private class CooldownAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;

            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
            [Opt] public readonly EcsTagPool<CreationChainTag> CreationChainTag;
            [Opt] public readonly EcsPool<TargetEntity> Target;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<AnimalTag> _animalTag;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LevelAspect _))
            {
                foreach (int entity in _world.Where(out ChainAspect aspect))
                {
                    CooldownAspect cooldownAspect = _world.GetAspect<CooldownAspect>();

                    EcsSpan animals = _world.Where(out AnimalAspect _);

                    for (int index = 0; index < animals.Count; index++)
                    {
                        int cooldown = _world.NewEntity();

                        cooldownAspect.Cooldowns.Add(cooldown).Duration =
                            aspect.Cooldowns.Read(entity).Duration * (index + 1);

                        cooldownAspect.Refresh.Add(cooldown);

                        aspect.CreationChain.Add(cooldown);

                        cooldownAspect.DeleteOnExpired.Add(cooldown);
                        cooldownAspect.Target.Add(cooldown).Value = _world.GetEntityLong(animals[index]);
                    }

                    aspect.Cooldowns.Get(entity).Duration *= 4;
                    cooldownAspect.Refresh.Add(entity);
                    aspect.DeleteOnExpired.Add(entity);
                    aspect.Target.Add(entity).Value = _world.GetEntityLong(level);
                }
            }
        }
    }
}