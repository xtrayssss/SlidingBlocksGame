using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateBlocksRequestingSystem : IEcsInit
    {
        [EcsInject] private EcsDefaultWorld _world;

        public void Init()
        {
            foreach (int entity in _world.Where(out SingleAspect<EcsTagPool<GameTag>> _))
            {
                _world.GetTagPool<CreateBlocksRequest>().Add(entity);
            }
        }
    }
}