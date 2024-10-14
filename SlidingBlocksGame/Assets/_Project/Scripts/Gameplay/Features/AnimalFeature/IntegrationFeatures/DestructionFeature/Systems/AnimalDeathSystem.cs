using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Systems
{
    public class AnimalDeathSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class DyingAnimals : EcsAspectAuto
        {
            [IncImplicit(typeof(DeathEvent))]
            [IncImplicit(typeof(AnimalTag))]
            [Opt] public readonly EcsTagPool<DeleteEntityRequest> DeleteEntity;
        }

        public void Run()
        {
            foreach (int animal in _world.Where(out DyingAnimals animalAspect))
                animalAspect.DeleteEntity.TryAdd(animal);
        }
    }
}