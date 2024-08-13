using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
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
            int game = _world.NewEntity(_gameCfg);

            _world.GetPool<LevelIndex>().Add(game);
            _world.GetPool<GameCreatedEvent>().Add(game);
        }
    }
}