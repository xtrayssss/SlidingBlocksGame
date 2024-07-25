using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateGameSystem : IEcsInit
    {
        [EcsInject] private EcsDefaultWorld _world;

        private readonly ITemplate _gameCfg;

        public CreateGameSystem(ITemplate gameCfg) =>
            _gameCfg = gameCfg;

        public void Init()
        {
            int entity = _world.NewEntity(_gameCfg);
            
            _world.GetTagPool<CreateGameRequest>().Add(entity);
        }
    }
}