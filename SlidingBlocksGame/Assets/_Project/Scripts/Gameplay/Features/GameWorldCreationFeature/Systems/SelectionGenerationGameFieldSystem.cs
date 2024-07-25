using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class SelectionGenerationGameFieldSystem : IEcsInit
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AlgorithmGenerationGameField> Algorithms;
        }

        public void Init()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly AlgorithmGenerationGameField algorithm = ref aspect.Algorithms.Read(entity);

                switch (algorithm.Value)
                {
                    case AlgorithmGenerationGameField.ID.None:
                        _world.GetTagPool<GenerateGameFieldRequest>().Add(entity);
                        break;
                    case AlgorithmGenerationGameField.ID.Wave:
                        _world.GetTagPool<GenerateWaveGameFieldRequest>().Add(entity);
                        break;
                    case AlgorithmGenerationGameField.ID.SmoothnessWave:
                        _world.GetTagPool<GenerateSmoothnessWaveGameFieldRequest>().Add(entity);
                        break;
                    case AlgorithmGenerationGameField.ID.Random:
                        break;
                }
            }
        }
    }
}