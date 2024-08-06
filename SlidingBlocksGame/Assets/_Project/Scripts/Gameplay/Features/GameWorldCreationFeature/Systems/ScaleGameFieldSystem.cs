using System;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class ScaleGameFieldSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GenerateGameFieldRequest))]
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly GameField gameField = ref aspect.GameFields.Read(entity);

                foreach (ref GameField.Cell cell in gameField.Cells.AsSpan())
                {
                    cell.View.transform.localScale = new Vector3(gameField.CellSize, cell.View.transform.localScale.y,
                        gameField.CellSize);
                }
                
                foreach (ref GameField.Unit unit in gameField.Units.AsSpan())
                {
                    unit.View.transform.localScale = new Vector3(gameField.CellSize, gameField.CellSize,
                        gameField.CellSize);
                }
            }
        }
    }
}