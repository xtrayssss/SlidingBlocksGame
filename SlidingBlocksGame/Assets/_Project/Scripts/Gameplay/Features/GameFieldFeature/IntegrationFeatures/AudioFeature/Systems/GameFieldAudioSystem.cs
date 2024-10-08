using _Project.Scripts.Gameplay.Features.AudioBaseFeature;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.IntegrationFeatures.AudioFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.IntegrationFeatures.AudioFeature.Systems
{
    public class GameFieldAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class TileGeneratedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(TileGeneratedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class GameFieldGeneratedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGeneratedEvent))]
            [Inc] public readonly EcsPool<GameFieldGeneratedAudioConfig> AudioConfigs;
        }

        private class TargetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TileGeneratedAudioConfig> AudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GameFieldGeneratedAspect aspect))
            {
                Debug.Log("GameFieldAudio");

                _world.NewAudioEntity(aspect.AudioConfigs.Read(entity).Value);
            }

            foreach (int entity in _world.Where(out TileGeneratedAspect tileGeneratedAspect))
            {
                TargetAspect targetAspect = _world.GetAspect<TargetAspect>();

                if (!tileGeneratedAspect.Targets.Read(entity).Value.TryGetID(out int targetID) ||
                    !targetAspect.IsMatches(targetID))
                    return;

                Debug.Log("TileAudio");

                _world.NewAudioEntity(targetAspect.AudioConfigs.Read(targetID).Value);
            }
        }
    }
}