using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateGameSystem : IEcsInit
    {
        [EcsInject] private EcsDefaultWorld _world;

        private readonly ITemplate _gameCfg;

        private class GameAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<PlayerCfgRef> PlayerCfg;

            [Opt] public readonly EcsTagPool<GameCreatedEvent> GameCreatedEvent;
        }

        public CreateGameSystem(ScriptableEntityTemplate gameCfg) =>
            _gameCfg = gameCfg;

        public void Init()
        {
            int game = _world.NewEntity(_gameCfg);

            GameAspect gameAspect = _world.GetAspect<GameAspect>();

            gameAspect.GameCreatedEvent.Add(game);

            _world.NewEntity(gameAspect.PlayerCfg.Read(game).Value);
        }
    }
}