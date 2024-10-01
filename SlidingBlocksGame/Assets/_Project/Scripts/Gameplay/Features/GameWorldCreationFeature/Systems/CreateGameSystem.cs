using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
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

            ref Levels levels = ref gameAspect.Levels.Get(game);

            levels.Randoms = new int[levels.Pack.Length][];

            for (int i = 0; i < levels.Pack.Length; i++)
            {
                levels.Randoms[i] = new int[levels.Pack[i].Levels.Length];

                for (int j = 0; j < levels.Randoms[i].Length; j++)
                    levels.Randoms[i][j] = j;

                for (int k = levels.Randoms[i].Length - 1; k > 0; k--)
                {
                    int randomIndex = Random.Range(0, k + 1);

                    (levels.Randoms[i][k], levels.Randoms[i][randomIndex]) =
                        (levels.Randoms[i][randomIndex], levels.Randoms[i][k]);
                }

#if UNITY_EDITOR
                Debug.Log($"Pack {i} levels after shuffle: {string.Join(", ", levels.Randoms[i])}");
#endif
            }

            if (gameAspect.PlayerCfg.Has(game))
                CreatePlayer(gameAspect, game);
        }

        private void CreatePlayer(GameAspect gameAspect, int game) =>
            _world.NewEntity(gameAspect.PlayerCfg.Read(game).Value);
    }
}