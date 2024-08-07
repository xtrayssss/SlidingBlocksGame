using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class DestructionChainStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ChainAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Chain> Chains;
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
            [Inc] public readonly EcsTagPool<DestructionChainTag> DestructionChainTag;
        }

        private class CooldownAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;

            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
            [Opt] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpired;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out ChainAspect chainAspect))
            {
                ref readonly Chain chain = ref chainAspect.Chains.Read(entity);

                EcsLongsSpan chainSpan = chain.Value.Longs;

                CooldownAspect cooldownAspect = _world.GetAspect<CooldownAspect>();

                for (int index = 0; index < chainSpan.Count; index++)
                {
                    entlong segment = chainSpan[index];

                    if (segment.TryGetID(out _))
                    {
                        int cooldown = _world.NewEntity();
                        
                        cooldownAspect.Cooldowns.Add(cooldown).Duration =
                            chainAspect.Cooldowns.Read(entity).Duration * (index + 1);
                        
                        cooldownAspect.Refresh.Add(cooldown);
                        
                        chainAspect.DestructionChainTag.Add(cooldown);
                        
                        _world.GetPool<DeleteOnExpiredMarker>().Add(cooldown);
                        _world.GetPool<TargetEntity>().Add(cooldown).Value = segment;
                    }
                }
                
                chainAspect.DestructionChainTag.Del(entity);
            }
        }
    }
}