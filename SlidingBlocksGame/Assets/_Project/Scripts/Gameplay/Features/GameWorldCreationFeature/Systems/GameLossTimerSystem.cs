using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GameLossTimerSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalPositionedEvent))]
            [Inc] public readonly EcsPool<GameLossTimerCfg> TimerConfigs;

            [Inc] public readonly EcsPool<HUD> HUD;
        }

        private class TimerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Prefab> Prefabs;

            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
        }

        private class HUDAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                entlong timer = _world.NewEntityLong(aspect.TimerConfigs.Read(entity).Value);

                TimerAspect timerAspect = _world.GetAspect<TimerAspect>();

                if (timerAspect.IsMatches(timer.ID))
                {
                    if (aspect.HUD.Read(entity).Value.TryGetID(out int hudID))
                    {
                        HUDAspect hudAspect = _world.GetAspect<HUDAspect>();

                        if (hudAspect.IsMatches(hudID))
                        {
                            EcsEntityConnect connect = Object.Instantiate(
                                original: timerAspect.Prefabs.Read(timer.ID).Value,
                                parent: hudAspect.GameObjectConnects.Read(hudID).Connect.transform,
                                worldPositionStays: false);

                            connect.Connect(timer, false);
                            
                            foreach (MonoEntityTemplateBase template in connect.MonoTemplates)
                                template.Apply(_world.id, timer.ID);

                            timerAspect.Refresh.Add(timer.ID);
                        }
                    }
                }
            }
        }
    }
}