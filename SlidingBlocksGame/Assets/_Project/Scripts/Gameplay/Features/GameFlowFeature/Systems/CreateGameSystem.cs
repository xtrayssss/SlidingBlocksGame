using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class CreateGameSystem : IEcsInit
    {
        [EcsInject] private EcsDefaultWorld _world;

        private readonly ITemplate _gameCfg;

        private class GameAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<PlayerCfgRef> PlayerCfg;
            [Opt] public readonly EcsTagPool<GameCreatedEvent> GameCreatedEvent;
            [Inc] public readonly EcsPool<Levels> Levels;
        }

        public CreateGameSystem(ScriptableEntityTemplate gameCfg) =>
            _gameCfg = gameCfg;

        public void Init()
        {
            int game = _world.NewEntity(_gameCfg);

            GameAspect gameAspect = _world.GetAspect<GameAspect>();

            gameAspect.GameCreatedEvent.Add(game);
            
            if (gameAspect.PlayerCfg.Has(game))
                CreatePlayer(gameAspect, game);
        }

        private void CreatePlayer(GameAspect gameAspect, int game) =>
            _world.NewEntity(gameAspect.PlayerCfg.Read(game).Value);
    }
}