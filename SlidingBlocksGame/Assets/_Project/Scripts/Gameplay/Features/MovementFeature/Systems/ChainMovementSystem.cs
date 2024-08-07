using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class ChainMovementSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CooldownAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
        }

        private class ChainAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ChainMovementMarker))]
            [Inc] public readonly EcsPool<ChainMovementCooldown> Cooldowns;

            [Inc] public readonly EcsPool<Chain> Chains;

            [Opt] public readonly EcsTagPool<ChainMovementMarker> ChainMovementMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out ChainAspect chainAspect))
            {
                EcsLongsSpan value = chainAspect.Chains.Read(entity).Value.Longs;

                for (int index = 0; index < value.Count; index++)
                {
                    entlong segment = value[index];

                    if (segment.TryUnpack(out _, out short _))
                    {
                        int cooldown = _world.NewEntity();

                        CooldownAspect cooldownAspect = _world.GetAspect<CooldownAspect>();

                        cooldownAspect.Cooldowns.Add(cooldown).Duration =
                            chainAspect.Cooldowns.Read(entity).Duration * index;
                        cooldownAspect.Refresh.Add(cooldown);

                        chainAspect.ChainMovementMarker.Add(cooldown);

                        _world.GetPool<DeleteOnExpiredMarker>().Add(cooldown);
                        _world.GetPool<TargetEntity>().Add(cooldown).Value = segment;
                    }
                }
            }
        }
    }
}