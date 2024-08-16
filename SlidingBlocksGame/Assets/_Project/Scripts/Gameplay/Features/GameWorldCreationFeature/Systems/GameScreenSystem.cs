using System.Linq;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GameScreenSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameCreatedEvent))]
            [Inc] public readonly EcsPool<GameScreenCfg> GameScreenConfigs;

            [Opt] public readonly EcsPool<GameScreen> GameScreen;
        }

        private class GameScreenAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Prefab> Prefabs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                entlong screen = _world.NewEntityLong(aspect.GameScreenConfigs.Read(entity).Value);

                GameScreenAspect screenAspect = _world.GetAspect<GameScreenAspect>();

                if (screenAspect.IsMatches(screen.ID))
                {
                    EcsEntityConnect connect = Object.Instantiate(screenAspect.Prefabs.Read(screen.ID).Value);

                    connect.Connect(screen, false);

                    foreach (MonoEntityTemplateBase template in connect.MonoTemplates)
                        template.Apply(_world.id, screen.ID);

                    aspect.GameScreen.TryAddOrGet(entity).Value = screen;

                    CreateBest(connect);
                }
            }
        }

        private void CreateBest(EcsEntityConnect connect)
        {
            entlong screen = _world.NewEntityLong();

            // TODO: 
            EcsEntityConnect bestConnect = connect.transform.GetChild(0).Find("Best").
                GetComponent<EcsEntityConnect>();

            bestConnect.Connect(screen, true);
        }
    }
}