using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class ChainDestructionRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredMarker))]
            [IncImplicit(typeof(DestructionChainTag))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteEntityCommand;
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyViewRequest;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    if (_world.GetPool<GameObjectConnect>().Has(targetID))
                        aspect.DestroyViewRequest.TryAdd(targetID);

                    aspect.DeleteEntityCommand.Add(targetID);
                }
            }
        }
    }
}