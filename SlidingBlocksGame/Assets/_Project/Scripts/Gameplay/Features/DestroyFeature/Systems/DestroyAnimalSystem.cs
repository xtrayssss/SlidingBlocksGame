using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class DestroyAnimalSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestroyUnitRequest))]
            [IncImplicit(typeof(AnimalTag))]
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyViewRequest;
            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteEntityCommand;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                aspect.DestroyViewRequest.Add(entity);
                aspect.DeleteEntityCommand.Add(entity);
            }
        }
    }
}