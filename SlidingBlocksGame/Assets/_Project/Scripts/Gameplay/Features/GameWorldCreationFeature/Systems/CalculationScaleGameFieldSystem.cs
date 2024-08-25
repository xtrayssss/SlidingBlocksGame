using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CalculationScaleGameFieldSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (var entity in _world.Where(out Aspect aspect))
            {
                ref GameField gameField = ref aspect.GameFields.Get(entity);

                gameField.Cells = new GameField.Cell[gameField.CellsCount];

                float initialZoneSize = gameField.BaseSize * gameField.BaseCellSize;

                float newTileSize = initialZoneSize / gameField.Size;

                Debug.Log(newTileSize);
                gameField.CellSize = newTileSize;
            }
        }
    }
}