using DCFApixels.DragonECS;
using UnityEngine;

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
            _world.NewEntity(_gameCfg);
        }
    }
}