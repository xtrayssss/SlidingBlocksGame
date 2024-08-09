using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateAnimalsRequestSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        public void Run()
        {
            foreach (int entity in _world.Where(out SingleAspect<EcsTagPool<CreateLevelRequest>> _))
            {
                _world.GetTagPool<Components.CreateAnimalsRequest>().Add(entity);
            }
        }
    }
}