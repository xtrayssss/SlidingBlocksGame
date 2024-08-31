using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class GameFieldAudioRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private readonly AudioUtils _audioUtils;

        public GameFieldAudioRequestSystem(AudioUtils audioUtils) =>
            _audioUtils = audioUtils;

        private class GameFieldGeneratedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGeneratedEvent))]
            [Opt] public readonly EcsTagPool<GameFieldGeneratedWaveAlgorithmMarker> GameFieldGeneratedWaveAlgorithm;

            [Opt] public readonly EcsTagPool<GameFieldGeneratedPlaneAlgorithmMarker> GameFieldGeneratedPlaneAlgorithm;

            [Opt] public readonly EcsPool<GameFieldGeneratedWaveAlgorithmAudioConfig>
                GameFieldGeneratedWaveAlgorithmAudioConfigs;

            [Opt] public readonly EcsPool<GameFieldGeneratedPlaneAlgorithmAudioConfig>
                GameFieldGeneratedPlaneAlgorithmAudioConfigs;
        }

        private class GameFieldGeneratedProgressAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<GameFieldGeneratedWaveAlgorithmMarker> GameFieldGeneratedWaveAlgorithm;

            [Opt] public readonly EcsPool<TileGeneratedWaveAlgorithmAudioConfig>
                TileGeneratedWaveAlgorithmAudioConfigs;
        }

        private class SpawnedRetranslationAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(TileGeneratedEvent))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GameFieldGeneratedAspect aspect))
            {
                if (aspect.GameFieldGeneratedWaveAlgorithm.Has(entity))
                    _audioUtils.Create(aspect.GameFieldGeneratedWaveAlgorithmAudioConfigs.Read(entity).Value);

                if (aspect.GameFieldGeneratedPlaneAlgorithm.Has(entity))
                    _audioUtils.Create(aspect.GameFieldGeneratedPlaneAlgorithmAudioConfigs.Read(entity).Value);
            }

            foreach (int entity in _world.Where(out SpawnedRetranslationAspect spawnedRetranslationAspect))
            {
                if (spawnedRetranslationAspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    GameFieldGeneratedProgressAspect gameFieldGeneratedProgressAspect =
                        _world.GetAspect<GameFieldGeneratedProgressAspect>();

                    if (gameFieldGeneratedProgressAspect.IsMatches(targetID))
                    {
                        _audioUtils.Create(gameFieldGeneratedProgressAspect.TileGeneratedWaveAlgorithmAudioConfigs
                            .Read(targetID).Value);
                    }
                }
            }
        }
    }
}