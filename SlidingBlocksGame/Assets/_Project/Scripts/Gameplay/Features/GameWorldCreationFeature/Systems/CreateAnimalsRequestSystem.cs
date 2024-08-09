using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateAnimalsRequestSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<GameFieldGeneratedEvent> _gameFieldGeneratedMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect _)) 
                _world.GetTagPool<CreateAnimalsRequest>().Add(entity);
        }
    }
}