using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateBlocksRequestingSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] private EcsTagPool<CreateGameRequest> _; 
            //[Inc] private EcsTagPool<GameTag> _1; 
        }
        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect _))
            {
                _world.GetTagPool<CreateBlocksRequest>().Add(entity);
            }
        }
    }
}