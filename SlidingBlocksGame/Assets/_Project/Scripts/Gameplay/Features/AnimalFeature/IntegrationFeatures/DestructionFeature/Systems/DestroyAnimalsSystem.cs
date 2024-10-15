using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Systems
{
    public class DestroyAnimalsSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class DyingAnimals : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<AnimalTag> AnimalTag;
            [Inc] public readonly EcsTagPool<DiedEvent> DiedEvent;
        }

        public void Run()
        {
            foreach (int animal in _world.Where(out DyingAnimals _)) 
                _world.DelEntity(animal);
        }
    }
}