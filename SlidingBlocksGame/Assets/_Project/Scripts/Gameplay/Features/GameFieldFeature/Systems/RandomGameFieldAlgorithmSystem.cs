using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
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
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly GameFieldAlgorithms algorithms = ref aspect.Algorithms.Read(entity);

                uint randomIndex = (uint)Random.Range(0, (uint)algorithms.Value.Length);

                int pack = _world.NewEntity(algorithms.Value[randomIndex]);

                AlgorithmsAspect algorithmsAspect = _world.GetAspect<AlgorithmsAspect>();

                ref readonly GameFieldAlgorithmCfg algorithmCfg = ref algorithmsAspect.Algorithms.Read(pack);
                int algorithm = _world.NewEntity(algorithmCfg.Value);

                AlgorithmAspect algorithmAspect = _world.GetAspect<AlgorithmAspect>();
                algorithmAspect.GameFieldGenerateRequest.Add(algorithm);
                algorithmAspect.Target.Add(algorithm).Value = _world.GetEntityLong(entity);

                aspect.GameFieldAlgorithmIndex.Add(entity).Value = randomIndex;
            }
        }
    }
}