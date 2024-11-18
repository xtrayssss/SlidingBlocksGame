using System;
using System.Linq;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Extensions;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class CalculateDestinationSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovableMarker))]
            [ExcImplicit(typeof(MovingMarker))]
            [Inc] public readonly EcsPool<MovementDirection> Directions;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;

            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Opt] public readonly EcsPool<CellDestination> CellDestination;
            [Opt] public readonly EcsPool<WorldDestination> WorldDestination;
        }

        private class ClickAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(SideClickedEvent))]
            [Inc] public readonly EcsPool<WorldPosition> WorldPositions;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Inc] public readonly EcsPool<MovementStrategyCfg> MovementStrategyConfigs;
        }

        public void Run()
        {
            foreach (int click in _world.Where(out ClickAspect clickAspect))
            {
                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                if (!clickAspect.ActiveGameFields.Read(click).Value.TryGetID(out int gameFieldID) ||
                    !gameFieldAspect.IsMatches(gameFieldID))
                    continue;

                ref readonly WorldPosition clickPosition = ref clickAspect.WorldPositions.Read(click);
                ref GameField gameField = ref gameFieldAspect.GameFields.Get(gameFieldID);

                float2 worldToGridPosition = GridUtils.GetCellPosition(clickPosition.Value, gameField.ToGrid());
                int2 moveDirection = GridUtils.GetInvertedSide(
                    position: worldToGridPosition,
                    edgeSize: gameField.EdgeSize,
                    centerSize: gameField.CenterSize);

                if (math.all(moveDirection == 0))
                    continue;

                // Собираем группу животных для перемещения
                var animals = EcsGroup.New(_world);
                var positions = new System.Collections.Generic.List<int2>();

                foreach (int animal in _world.Where(out AnimalAspect animalAspect))
                {
                    if (math.all(animalAspect.Directions.Read(animal).Value == moveDirection))
                    {
                        animals.Add(animal);
                        positions.Add(_world.GetPool<CellPosition>().Read(animal).Value);
                    }
                }

                if (animals.Count == 0)
                    continue;

                // Проверяем возможность движения для всей группы
                int maxMovement = CalculateGroupMovement(
                    currentPositions: positions,
                    moveDirection: moveDirection,
                    gameField: gameField);

                // Если движение невозможно (maxMovement == 0), пропускаем обработку
                // Применяем движение ко всем животным
                foreach (int animal in animals)
                {
                    var currentPos = _world.GetPool<CellPosition>().Read(animal).Value;
                    var targetPos = currentPos + maxMovement * moveDirection;

                    ref var cellDest = ref _world.GetPool<CellDestination>().Add(animal);
                    cellDest.Value = targetPos;

                    float3 worldDest = GridUtils.GetWorldPosition(targetPos, gameField.ToGrid());
                    _world.GetPool<WorldDestination>().Add(animal).Value = new float3(
                        worldDest.x,
                        _world.GetPool<GameObjectConnect>().Read(animal).Connect.transform.position.y,
                        worldDest.z);

                    GridUtils.SetCell(
                        position: targetPos,
                        edgeSize: gameField.EdgeSize,
                        grid: ref gameField.Center);
                }

                // Apply movement strategy
                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    ref readonly MovementStrategyCfg cfg = ref gameAspect.MovementStrategyConfigs.Read(game);
                    int strategy = _world.NewEntity(cfg.Value);
                    _world.GetPool<ApplyMovementStrategyRequest>().Add(strategy);
                    _world.GetPool<TargetEntities>().Add(strategy).Value = animals;
                }
            }
        }

        private int CalculateGroupMovement(
            System.Collections.Generic.List<int2> currentPositions,
            int2 moveDirection,
            in GameField gameField)
        {
            // Определяем границу центральной области
            int2 centerBoundary;
            if (math.any(moveDirection < 0))
                centerBoundary = new int2(gameField.EdgeSize);
            else
                centerBoundary = new int2(gameField.EdgeSize + gameField.CenterSize - 1);

            // Находим максимально возможное расстояние до центра
            int maxDistance = int.MaxValue;
            foreach (var pos in currentPositions)
            {
                int distance;
                if (moveDirection.x != 0)
                {
                    distance = math.abs(centerBoundary.x - pos.x);
                }
                else
                {
                    distance = math.abs(centerBoundary.y - pos.y);
                }
 
                maxDistance = math.min(maxDistance, distance);
            }

            // Проверяем каждый шаг движения для всей группы
            for (int step = 1; step <= maxDistance; step++)
            {
                // Проверяем, не займет ли какое-либо животное уже занятую клетку
                bool collision = false;
                foreach (var currentPos in currentPositions)
                {
                    int2 nextPos = currentPos + moveDirection * step;

                    // Проверяем, находится ли позиция в центральной области
                    if (GridUtils.IsWithinCenter(nextPos, gameField.EdgeSize, gameField.CenterSize))
                    {
                        int2 dimensions = nextPos - new int2(gameField.EdgeSize);
                        int bitPosition = dimensions.y * gameField.CenterSize + dimensions.x;

                        // Если клетка уже занята
                        if ((gameField.Center & (1 << bitPosition)) != 0)
                        {
                            collision = true;
                            break;
                        }
                    }
                }

                // Если обнаружена коллизия, возвращаем предыдущий безопасный шаг
                if (collision)
                {
                    return step - 1;
                }
            }

            return maxDistance;
        }
    }
}