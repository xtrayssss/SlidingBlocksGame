using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateHUDSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameCreatedEvent))]
            [Inc] public readonly EcsPool<HUDPrefab> HUDPrefabs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                EcsEntityConnect connect = Object.Instantiate(aspect.HUDPrefabs.Read(entity).Value);

                entlong hud = _world.NewEntityLong();

                connect.Connect(hud, applyTemplates: true);

                CreateBest(connect);
            }
        }

        private void CreateBest(EcsEntityConnect connect)
        {
            entlong screen = _world.NewEntityLong();

            // TODO: 
            EcsEntityConnect bestConnect = connect.transform.Find("Best").GetComponent<EcsEntityConnect>();

            bestConnect.Connect(screen, true);
        }
    }
}