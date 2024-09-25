using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Systems
{
    public class CalculateCellScaleYSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref GameField gameField = ref aspect.GameFields.Get(entity);

                gameField.CellScaleY = gameField.BaseCellScaleY *
                                              ((float)gameField.BaseSize / gameField.Size);
            }
        }
    }
}