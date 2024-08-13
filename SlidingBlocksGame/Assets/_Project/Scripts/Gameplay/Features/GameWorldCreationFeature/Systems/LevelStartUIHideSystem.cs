using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class LevelStartUIHideSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateLevelRequest))]
            [Inc] public readonly EcsPool<GameScreen> GameScreen;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<HideUI> Hiddens;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.GameScreen.Read(entity).Value.TryGetID(out int gameScreenID))
                {
                    GameScreenAspect gameScreenAspect = _world.GetAspect<GameScreenAspect>();
                    
                    if (gameScreenAspect.IsMatches(gameScreenID))
                    {
                        ref readonly HideUI hiddens = ref gameScreenAspect.Hiddens.Read(gameScreenID);
                        
                        foreach (GameObject ui in hiddens.Value)
                            ui.SetActive(false);
                    }
                }
            }
        }
    }
}