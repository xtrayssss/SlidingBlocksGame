using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class LevelStartRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class EventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> _0;
            [Inc] public readonly EcsTagPool<PlayButtonTag> _;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevel;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out EventAspect _))
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    gameAspect.NextLevel.Add(game);
                }
            }
        }
    }
}