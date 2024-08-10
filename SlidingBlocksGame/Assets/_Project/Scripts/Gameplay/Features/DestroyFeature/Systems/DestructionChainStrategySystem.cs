using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class DestructionChainStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ChainAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ApplyDestructionStrategyRequest))]
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            [Inc] public readonly EcsTagPool<DestructionChainTag> DestructionChainTag;
        }

        private class CooldownAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;

            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
            [Opt] public readonly EcsPool<TargetEntity> Target;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [ExcImplicit(typeof(MovingMarker))]
            private int _;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out ChainAspect chainAspect))
            {
                CooldownAspect cooldownAspect = _world.GetAspect<CooldownAspect>();

                EcsSpan animals = _world.Where(out AnimalAspect _);

                for (int index = 0; index < animals.Count; index++)
                {
                    int animal = animals[index];

                    int cooldown = _world.NewEntity();

                    cooldownAspect.Cooldowns.Add(cooldown).Duration =
                        chainAspect.Cooldowns.Read(entity).Duration * (index + 1);

                    cooldownAspect.Refresh.Add(cooldown);

                    chainAspect.DestructionChainTag.Add(cooldown);

                    cooldownAspect.Target.Add(cooldown).Value = _world.GetEntityLong(animal);
                }

                Debug.Log(animals.Count);
                
                cooldownAspect.Cooldowns.Get(entity).Duration *= animals.Count;
                cooldownAspect.Refresh.Add(entity);
                cooldownAspect.DeleteOnExpired.Add(entity);
            }
        }
    }
}