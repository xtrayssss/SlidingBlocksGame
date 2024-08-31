using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class ChainDestructionRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(DestructionChainTag))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteEntityCommand;
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyViewRequest;
            [Opt] public readonly EcsTagPool<DeathEvent> Death;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Opt] public readonly EcsTagPool<AnimalDestructedEvent> AnimalDestructed;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int targetID) &&
                    _world.GetAspect<AnimalAspect>().IsMatches(targetID))
                {
                    if (_world.GetPool<GameObjectConnect>().Has(targetID)) 
                        aspect.DestroyViewRequest.TryAdd(targetID);

                    aspect.DeleteEntityCommand.Add(targetID);
                    aspect.Death.Add(targetID);
                }
            }

            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    LevelAspect levelAspect = _world.GetAspect<LevelAspect>();

                    if (levelAspect.IsMatches(targetID))
                    {
                        levelAspect.AnimalDestructed.Add(targetID);
                    }
                }
            }
        }
    }
}