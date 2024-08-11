using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class MainMenuSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<MainMenuCfg> MainMenuConfigs;
        }

        private class MainMenuAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Prefab> Prefabs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                entlong menu = _world.NewEntityLong(aspect.MainMenuConfigs.Read(entity).Value);

                MainMenuAspect hudAspect = _world.GetAspect<MainMenuAspect>();

                if (hudAspect.IsMatches(menu.ID))
                {
                    EcsEntityConnect connect = Object.Instantiate(hudAspect.Prefabs.Read(menu.ID).Value);

                    connect.Connect(menu, false);

                    foreach (MonoEntityTemplateBase template in connect.MonoTemplates)
                        template.Apply(_world.id, menu.ID);
                }
            }
        }
    }

    [Serializable]
    public struct MainMenuCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private sealed class Template : ComponentTemplate<MainMenuCfg>
        {
        }
    }

    public class HUDSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalPositionedEvent))]
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
                // TODO: rework
                //
                // if (GameObject.Find("HUD(Clone)")) 
                //     continue;

                entlong hud = _world.NewEntityLong(aspect.HUDConfigs.Read(entity).Value);

                HUDAspect hudAspect = _world.GetAspect<HUDAspect>();

                if (hudAspect.IsMatches(hud.ID))
                {
                    EcsEntityConnect connect = Object.Instantiate(hudAspect.Prefabs.Read(hud.ID).Value);

                    connect.Connect(hud, false);

                    foreach (MonoEntityTemplateBase template in connect.MonoTemplates)
                        template.Apply(_world.id, hud.ID);

                    aspect.HUD.TryAddOrGet(entity).Value = hud;
                }
            }
        }
    }
}