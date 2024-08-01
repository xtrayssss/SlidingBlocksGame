using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class DestinationUnavailabilityCheckRequestSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestinationUnavailabilityCheckRequest))]
            [ExcImplicit(typeof(DestinationUnavailableMarker))]
            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;

            [Inc] public readonly EcsPool<CellPosition> CellPositions;
            [Inc] public readonly EcsPool<MovementDirection> Directions;
        }

        private class RequestAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<DestinationUnavailabilityCheckRequest> OccupancyCheckRequest;
            [Opt] public readonly EcsPool<TargetEntity> Target;
            [Opt] public readonly EcsPool<ObstacleCellPosition> ObstaclePosition;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly var direction = ref aspect.Directions.Read(entity);

                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                if (aspect.ActiveGameFields.Read(entity).Value.TryGetID(out int gameFieldID))
                {
                    ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(gameFieldID);

                    ref readonly CellPosition cellPosition = ref aspect.CellPositions.Read(entity);

                    float2 end = cellPosition.Value + direction.Value * gameField.EdgeSize;

                    float2 start;

                    if (direction.Value.x == 1 || direction.Value.y == 1)
                    {
                        float2 abs = 0;

                        if (direction.Value.x == 1)
                        {
                            abs = new float2(1, 0);
                        }
                        else
                        {
                            abs = new float2(0, 1);
                        }

                        start = ((gameField.EdgeSize * abs - cellPosition.Value * abs) + cellPosition.Value);
                    }
                    else
                    {
                        float2 abs = 0;

                        if (direction.Value.x == -1)
                        {
                            abs = new float2(1, 0);
                        }
                        else
                        {
                            abs = new float2(0, 1);
                        }

                        start = ((gameField.EdgeSize + gameField.CenterSize - 1) * abs) - (cellPosition.Value * abs) +
                                cellPosition.Value;
                    }

                    float2 progress = start;

                    Debug.Log(start);
                    Debug.Log(end);

                    int i = 0;
                    Debug.Log(entity);

                    do
                    {
                        progress += direction.Value * i;

                        Debug.Log(progress);

                        int request = _world.NewEntity();

                        RequestAspect requestAspect = _world.GetAspect<RequestAspect>();

                        requestAspect.OccupancyCheckRequest.Add(request);
                        requestAspect.Target.Add(request).Value = entity.ToEntityLong(requestAspect.World);
                        requestAspect.ObstaclePosition.Add(request).Value = progress;

                        i++;
                    } while ((Vector2)progress != (Vector2)end);
                }
            }
        }
    }
}