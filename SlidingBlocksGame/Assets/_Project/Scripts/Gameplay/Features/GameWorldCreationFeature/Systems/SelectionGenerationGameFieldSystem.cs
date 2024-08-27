using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class SelectionGenerationGameFieldSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public readonly EcsPool<GenerationGameFieldAlgorithmCfg> Algorithms;
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

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                int algorithm = _world.NewEntity(aspect.Algorithms.Get(entity).Value);

                _world.GetPool<GameFieldGenerateRequest>().Add(algorithm);
                _world.GetPool<TargetEntity>().Add(algorithm).Value = _world.GetEntityLong(entity);
            }

            foreach (int entity in _world.Where(out AlgorithmGeneratedAspect aspect))
                aspect.DeleteEntity.Add(entity);

            foreach (int entity in _world.Where(out AlgorithmDestructedAspect aspect))
                aspect.DeleteEntity.Add(entity);
        }
    }
}