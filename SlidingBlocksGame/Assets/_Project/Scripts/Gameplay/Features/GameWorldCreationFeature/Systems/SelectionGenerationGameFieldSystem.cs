using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
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

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                int algorithm = _world.NewEntity(aspect.Algorithms.Get(entity).Value);
                
                _world.GetPool<GameFieldGenerateRequest>().Add(algorithm);
                _world.GetPool<TargetEntity>().Add(algorithm).Value = _world.GetEntityLong(entity);
            }
        }
    }
}