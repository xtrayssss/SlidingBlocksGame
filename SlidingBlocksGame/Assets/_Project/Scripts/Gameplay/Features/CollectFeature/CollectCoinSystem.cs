using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using PrimeTween;
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

            [Inc] public readonly EcsPool<MeshRendererRef> MeshRenderers;

            [Exc] public readonly EcsTagPool<CollectedMarker> CollectedMarker;

            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<Coins> Coins;
            [Opt] public readonly EcsTagPool<CoinCollectedEvent> CoinCollectedEvent;
            [Opt] public readonly EcsTagPool<CollectedEvent> CollectedEvent;
            [Opt] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteEntity;
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyView;
        }

        private class MovingAnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovingMarker))]
            [IncImplicit(typeof(AnimalTag))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Inc] public readonly EcsPool<MovementDirection> MovementDirections;
            [Inc] public readonly EcsPool<BoundExtents> BoundExtents;
        }

        public void Run()
        {
            foreach (int coin in _world.Where(out CoinAspect coinAspect))
            {
                foreach (int animal in _world.Where(out MovingAnimalAspect movingAnimalAspect))
                {
                    if (movingAnimalAspect.ActiveGameFields.Read(animal).Value.TryGetID(out int gameFieldID))
                    {
                        float3 transformPosition = movingAnimalAspect.GameObjectConnects.Read(animal).Connect.transform
                            .position;

                        ref readonly BoundExtents boundExtents = ref movingAnimalAspect.BoundExtents.Read(animal);
                        
                        int2 position = GridUtils.GetCellPosition(
                            worldPosition: transformPosition +
                                           movingAnimalAspect.MovementDirections.Read(animal).Value.xyy *
                                           boundExtents.Value,
                            gameField: in _world.GetPool<GameField>().Read(gameFieldID));

                        if (math.all(coinAspect.CellPositions.Read(coin).Value == position))
                        {
                            coinAspect.CollectedMarker.Add(coin);

                            int @event = _world.NewEntity();

                            coinAspect.CoinCollectedEvent.Add(@event);
                            coinAspect.Coins.Add(@event).Value = coinAspect.Coins.Read(coin).Value;
                            coinAspect.DeleteEntity.Add(@event);
                            coinAspect.CollectedEvent.Add(@event);
                            coinAspect.Targets.Add(@event).Value = coin.ToEntityLong(_world);

                            ref GameObjectConnect gameObjectConnect = ref coinAspect.GameObjectConnects.Get(coin);

                            Tween fadeTween = Tween.MaterialColor(coinAspect.MeshRenderers.Read(coin).Value.material,
                                Color.clear, 0.8f, Ease.OutQuint);

                            Tween positionTween = Tween.Position(gameObjectConnect.Connect.transform,
                                new Vector3(gameObjectConnect.Connect.transform.position.x, 9,
                                    gameObjectConnect.Connect.transform.position.y), 0.8f,
                                Ease.OutQuint);

                            Sequence.Create()
                                .Group(positionTween)
                                .Group(fadeTween)
                                .ChainCallback(gameObjectConnect.Connect, target =>
                                {
                                    if (target.Entity.TryGetID(out int coinID))
                                    {
                                        coinAspect.DeleteEntity.Add(coinID);
                                        coinAspect.DestroyView.Add(coinID);
                                    }
                                });
                        }
                    }
                }
            }
        }
    }
}