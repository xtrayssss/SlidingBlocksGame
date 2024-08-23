using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEditor;

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

            _world.GetPool<LevelCounter>().Add(game);
            _world.GetPool<GameCreatedEvent>().Add(game);

            // TODO: move to config
            int player = _world.NewEntity();
            
            _world.GetPool<PlayerTag>().Add(player);
            _world.GetPool<Balance>().Add(player).Value = 1000;
            _world.GetPool<SelectionAnimalID>().Add(player);
        }
    }
}