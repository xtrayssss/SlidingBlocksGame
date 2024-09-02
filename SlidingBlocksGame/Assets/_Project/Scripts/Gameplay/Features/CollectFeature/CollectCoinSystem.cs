using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CollectFeature
{
    public class CollectCoinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CoinAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Inc] public readonly EcsPool<Coins> Coins;
            [Opt] public readonly EcsTagPool<CoinCollectedEvent> CoinCollected;
            [Opt] public readonly EcsTagPool<CollectedEvent> Collected;
            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteEntity;
            [Opt] public readonly EcsPool<TargetEntity> Targets;
        }

        private class MovingAnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovingMarker))]
            [IncImplicit(typeof(AnimalTag))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Inc] public readonly EcsPool<RendererRef> Renderers;
            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Inc] public readonly EcsPool<MovementDirection> MovementDirections;
        }

        public void Run()
        {
            foreach (int coin in _world.Where(out CoinAspect coinAspect))
            {
                foreach (int animal in _world.Where(out MovingAnimalAspect movingAnimalAspect))
                {
                    if (movingAnimalAspect.ActiveGameFields.Read(animal).Value.TryGetID(out int gameFieldID))
                    {
                        float3 calculateVectorToMaxEdge = CalculateVectorToMaxEdge(movingAnimalAspect.Renderers
                            .Get(animal).Value
                            .GetComponent<MeshRenderer>());

                        float3 transformPosition = movingAnimalAspect.GameObjectConnects.Read(animal).Connect.transform
                            .position;

                        int2 position = GridUtils.GetCellPosition(
                            worldPosition: transformPosition +
                                           movingAnimalAspect.MovementDirections.Read(animal).Value.xyy *
                                           new float3(calculateVectorToMaxEdge.x, 0, calculateVectorToMaxEdge.z),
                            gameField: in _world.GetPool<GameField>().Read(gameFieldID));

                        if (math.all(coinAspect.CellPositions.Read(coin).Value == position))
                        {
                            coinAspect.DeleteEntity.Add(coin);

                            int @event = _world.NewEntity();

                            coinAspect.CoinCollected.Add(@event);
                            coinAspect.Coins.Add(@event).Value = coinAspect.Coins.Read(coin).Value;
                            coinAspect.DeleteEntity.Add(@event);
                            coinAspect.Collected.Add(@event);
                            coinAspect.Targets.Add(@event).Value = coin.ToEntityLong(_world);
                        }
                    }
                }
            }
        }

        private float3 CalculateVectorToMaxEdge(MeshRenderer meshFilter)
        {
            Bounds bounds = meshFilter.bounds;

            Vector3 vectorToMaxEdge = bounds.extents;

            return vectorToMaxEdge;
        }
    }
}