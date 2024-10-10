using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
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
        
        private class AlgorithmsAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameFieldAlgorithmCfg> Algorithms;
            [Opt] public readonly EcsPool<GameFieldGeneratedAudioConfig> GameFieldGeneratedAudioConfigs;
            [Opt] public readonly EcsPool<TileGeneratedAudioConfig> TileGeneratedAudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out AlgorithmsAspect algorithmsAspect))
            {
                if (algorithmsAspect.GameFieldGeneratedAudioConfigs.Has(entity))
                {
                    algorithmsAspect.GameFieldGeneratedAudioConfigs.Add(entity).Value =
                        algorithmsAspect.GameFieldGeneratedAudioConfigs.Read(entity).Value;
                }

                if (algorithmsAspect.TileGeneratedAudioConfigs.Has(entity))
                {
                    algorithmsAspect.TileGeneratedAudioConfigs.Add(entity).Value =
                        algorithmsAspect.TileGeneratedAudioConfigs.Read(entity).Value;
                }

                _world.DelEntity(entity);
            }

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