using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class HUDSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelGenerateMarker))]
            [Inc] public readonly EcsPool<HUDCfg> HUDConfigs;

            [Opt] public readonly EcsPool<HUD> HUD;
        }

        private class HUDAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Prefab> Prefabs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                entlong hud = _world.NewEntityLong(aspect.HUDConfigs.Read(entity).Value);

                HUDAspect hudAspect = _world.GetAspect<HUDAspect>();

                if (hudAspect.IsMatches(hud.ID))
                {
                    EcsEntityConnect connect = Object.Instantiate(hudAspect.Prefabs.Read(hud.ID).Value);

                    connect.Connect(hud, false);
                    
                    foreach (MonoEntityTemplateBase template in connect.MonoTemplates)
                        template.Apply(_world.id, hud.ID);

                    aspect.HUD.Add(entity).Value = hud;
                }
            }
        }
    }
}