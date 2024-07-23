using System.Collections.Generic;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay
{
    public class EcsRoot : MonoBehaviour
    {
        [SerializeField] private ScriptableEntityTemplate _gameCfg;

        private EcsPipeline _pipeline;
        private EcsDefaultWorld _world;

        public void Start()
        {
            EcsDefaultWorldSingletonProvider provider = EcsDefaultWorldSingletonProvider.Instance;

            provider.Set(_world = new EcsDefaultWorld());

            _pipeline = EcsPipeline.New()
                .Add(new CreateGameSystem(_gameCfg))
                .Add(new CreateGameFieldSystem())
                .AddUnityDebug(_world)
                .Inject(_world)
                .AutoInject()
                .BuildAndInit();
        }

        public void Update() =>
            _pipeline.Run();

        public void FixedUpdate() =>
            _pipeline.FixedRun();

        public void OnDestroy()
        {
            _pipeline.Destroy();
            _pipeline = null;

            _world.Destroy();
            _world = null;
        }
    }
}