using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.PauseFeature.Components;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.PauseFeature.Systems
{
    public class PauseSystem : IAlwaysRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<PausedMarker> PausedMarker;

            [Opt] public readonly EcsTagPool<PausedEvent> PausedEvent;
        }

        private class PauseRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PauseRequest> PauseRequest;
        }

        private class UnpauseRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<UnpauseRequest> UnpauseRequest;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out PauseRequestAspect _))
            {
                
                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    if (!gameAspect.PausedMarker.Has(game))
                    {
                        Time.timeScale = 0;
                        gameAspect.PausedEvent.Add(game);
                        gameAspect.PausedMarker.Add(game);
                    }
                }
            }

            foreach (int _ in _world.Where(out UnpauseRequestAspect _))
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    if (gameAspect.PausedMarker.Has(game))
                    {
                        Time.timeScale = 1;
                        gameAspect.PausedMarker.Del(game);
                    }
                }
            }
        }
    }
}