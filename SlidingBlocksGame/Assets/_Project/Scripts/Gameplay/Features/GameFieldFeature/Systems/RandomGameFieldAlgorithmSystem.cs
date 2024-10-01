using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Systems
{
    public class RandomGameFieldAlgorithmSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public readonly EcsPool<GameFieldAlgorithms> Algorithms;

            [Opt] public readonly EcsPool<GameFieldGeneratedAudioConfig> GameFieldGeneratedAudioConfig;
            [Opt] public readonly EcsPool<TileGeneratedAudioConfig> TileGeneratedAudioConfig;
            [Opt] public readonly EcsPool<GameFieldAlgorithmIndex> GameFieldAlgorithmIndex;
        }

        private class AlgorithmsAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameFieldAlgorithmCfg> Algorithms;
        }

        private class AlgorithmAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<GameFieldGenerateRequest> GameFieldGenerateRequest;
            [Opt] public readonly EcsPool<TargetEntity> Target;
            [Opt] public readonly EcsPool<GameFieldGeneratedAudioConfig> GameFieldGeneratedAudioConfigs;
            [Opt] public readonly EcsPool<TileGeneratedAudioConfig> TileGeneratedAudioConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly GameFieldAlgorithms algorithms = ref aspect.Algorithms.Read(entity);

                uint randomIndex = (uint)Random.Range(0, (uint)algorithms.Value.Length);

                int pack = _world.NewEntity(algorithms.Value[randomIndex]);

                AlgorithmsAspect algorithmsAspect = _world.GetAspect<AlgorithmsAspect>();

                int algorithm = _world.NewEntity(algorithmsAspect.Algorithms.Read(pack).Value);

                AlgorithmAspect algorithmAspect = _world.GetAspect<AlgorithmAspect>();
                algorithmAspect.GameFieldGenerateRequest.Add(algorithm);
                algorithmAspect.Target.Add(algorithm).Value = _world.GetEntityLong(entity);

                if (algorithmAspect.GameFieldGeneratedAudioConfigs.Has(pack))
                {
                    aspect.GameFieldGeneratedAudioConfig.Add(entity).Value =
                        algorithmAspect.GameFieldGeneratedAudioConfigs.Read(pack).Value;
                }

                if (algorithmAspect.TileGeneratedAudioConfigs.Has(pack))
                {
                    aspect.TileGeneratedAudioConfig.Add(entity).Value =
                        algorithmAspect.TileGeneratedAudioConfigs.Read(pack).Value;
                }

                aspect.GameFieldAlgorithmIndex.Add(entity).Value = randomIndex;

                _world.DelEntity(pack);
            }
        }
    }
}