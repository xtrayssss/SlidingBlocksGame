using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Extensions
{
    public static class Extensions
    {
        public static GridUtils.Grid ToGrid(this in GameField gameField)
        {
            return new GridUtils.Grid
            {
                CellSize = gameField.CellSize,
                OriginPosition = gameField.OriginPosition,
                Offset = gameField.Offset
            };
        }
    }
}