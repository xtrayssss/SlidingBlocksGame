using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Random = Unity.Mathematics.Random;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class SelectGameFieldAlgorithmSystem : IEcsRun
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

        private class AlgorithmGeneratedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldAlgorithmTag))]
            [IncImplicit(typeof(GameFieldGeneratedEvent))]
            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteEntity;
        }

        private class AlgorithmDestructedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldAlgorithmTag))]
            [IncImplicit(typeof(GameFieldDestructedEvent))]
            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteEntity;
        }

        private class AlgorithmAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<GameFieldGenerateRequest> GameFieldGenerate;
            [Opt] public readonly EcsPool<TargetEntity> Target;
            [Opt] public readonly EcsPool<GameFieldGeneratedAudioConfig> GameFieldGeneratedAudioConfigs;
            [Opt] public readonly EcsPool<TileGeneratedAudioConfig> TileGeneratedAudioConfigs;
        }

        private class GameFieldDestructAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldDestructRequest))]
            [Inc] public readonly EcsPool<GameFieldAlgorithms> Algorithms;

            [Inc] public readonly EcsPool<GameFieldAlgorithmIndex> AlgorithmIndices;

            [Opt] public readonly EcsPool<GameFieldGeneratedAudioConfig> GameFieldGeneratedAudioConfig;
            [Opt] public readonly EcsPool<TileGeneratedAudioConfig> TileGeneratedAudioConfig;
            [Opt] public readonly EcsPool<GameFieldAlgorithmIndex> GameFieldAlgorithmIndex;
            [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestruct;
        }

        private Random _random = Random.CreateFromIndex(0);

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly GameFieldAlgorithms algorithms = ref aspect.Algorithms.Read(entity);

                uint randomIndex = _random.NextUInt(0, (uint)algorithms.Value.Length);

                int pack = _world.NewEntity(algorithms.Value[randomIndex]);

                AlgorithmsAspect algorithmsAspect = _world.GetAspect<AlgorithmsAspect>();

                int algorithm = _world.NewEntity(algorithmsAspect.Algorithms.Read(pack).Value);

                AlgorithmAspect algorithmAspect = _world.GetAspect<AlgorithmAspect>();
                algorithmAspect.GameFieldGenerate.Add(algorithm);
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
            }

            foreach (int entity in _world.Where(out GameFieldDestructAspect gameFieldDestructAspect))
            {
                ref readonly GameFieldAlgorithms algorithms = ref gameFieldDestructAspect.Algorithms.Read(entity);
                ref readonly GameFieldAlgorithmIndex algorithmIndex =
                    ref gameFieldDestructAspect.AlgorithmIndices.Read(entity);

                int pack = _world.NewEntity(algorithms.Value[algorithmIndex.Value]);

                AlgorithmsAspect algorithmsAspect = _world.GetAspect<AlgorithmsAspect>();

                int algorithm = _world.NewEntity(algorithmsAspect.Algorithms.Read(pack).Value);

                AlgorithmAspect algorithmAspect = _world.GetAspect<AlgorithmAspect>();
                gameFieldDestructAspect.GameFieldDestruct.Add(algorithm);
                algorithmAspect.Target.Add(algorithm).Value = _world.GetEntityLong(entity);
            }

            foreach (int entity in _world.Where(out AlgorithmGeneratedAspect aspect))
                aspect.DeleteEntity.Add(entity);

            foreach (int entity in _world.Where(out AlgorithmDestructedAspect aspect))
                aspect.DeleteEntity.Add(entity);
        }
    }
}