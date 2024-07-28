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
            [Inc] public readonly EcsPool<MovementDirection> Directions;

            [Inc] public readonly EcsPool<CellDestination> CellDestinations;
            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;

            [Inc] public readonly EcsPool<CellPosition> CellPositions;
            [Opt] public readonly EcsTagPool<DestinationUnavailabilityCheckRequest> OccupancyCheckRequest;
            [Opt] public readonly EcsPool<TargetEntity> Targets;
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
                ref readonly var destination = ref aspect.CellDestinations.Read(entity);

                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                if (aspect.ActiveGameFields.Read(entity).Value.TryGetID(out int id) &&
                    gameFieldAspect.IsMatches(id))
                {
                    ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(id);

                    CellPosition cellPosition = aspect.CellPositions.Read(entity);

                    float2 end = destination.Value;

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

                    float2 progress = default;

                    int i = 0;

                    
                    for (; !math.all(progress == end); i++)
                    {
                        progress = start + direction.Value * i;

                        int request = _world.NewEntity();

                        aspect.OccupancyCheckRequest.Add(request);
                        aspect.Targets.Add(request).Value = entity.ToEntityLong(aspect.World);
                        aspect.CellDestinations.Add(request).Value = progress;
                    }
                }
            }
        }
    }
}