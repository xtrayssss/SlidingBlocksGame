using System.Collections.Generic;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Extensions;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;

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
            [Opt] public readonly EcsPool<ProcessedSides> ProcessedSides;
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

                ref ProcessedSides processedSides = ref gameFieldAspect.ProcessedSides.TryAddOrGet(gameFieldID);

                processedSides.Value ??= new List<int2>(capacity: 4);

                if (math.all(moveDirection == 0) || processedSides.Value.Contains(moveDirection))
                    continue;

                processedSides.Value.Add(moveDirection);

                EcsGroup animals = EcsGroup.New(_world);
                List<int2> positions = new List<int2>();

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

                int maxMovement = CalculateGroupMovement(
                    currentPositions: positions,
                    moveDirection: moveDirection,
                    gameField: gameField);

                foreach (int animal in animals)
                {
                    int2 currentPos = _world.GetPool<CellPosition>().Read(animal).Value;
                    int2 targetPos = currentPos + maxMovement * moveDirection;

                    ref CellDestination cellDest = ref _world.GetPool<CellDestination>().Add(animal);
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

        private static int CalculateGroupMovement(
            List<int2> currentPositions,
            int2 moveDirection,
            in GameField gameField)
        {
            int2 centerBoundary;

            if (math.any(moveDirection < 0))
                centerBoundary = new int2(gameField.EdgeSize);
            else
                centerBoundary = new int2(gameField.EdgeSize + gameField.CenterSize - 1);

            int maxDistance = int.MaxValue;

            foreach (int2 pos in currentPositions)
            {
                int distance = moveDirection.x != 0
                    ? math.abs(centerBoundary.x - pos.x)
                    : math.abs(centerBoundary.y - pos.y);

                maxDistance = math.min(maxDistance, distance);
            }

            for (int step = 1; step <= maxDistance; step++)
            {
                bool collision = false;
                
                foreach (int2 currentPos in currentPositions)
                {
                    int2 nextPos = currentPos + moveDirection * step;

                    if (GridUtils.IsWithinCenter(nextPos, gameField.EdgeSize, gameField.CenterSize))
                    {
                        int2 dimensions = nextPos - new int2(gameField.EdgeSize);
                        int bitPosition = dimensions.y * gameField.CenterSize + dimensions.x;

                        if ((gameField.Center & (1 << bitPosition)) != 0)
                        {
                            collision = true;
                            break;
                        }
                    }
                }

                if (collision)
                    return step - 1;
            }

            return maxDistance;
        }
    }
}