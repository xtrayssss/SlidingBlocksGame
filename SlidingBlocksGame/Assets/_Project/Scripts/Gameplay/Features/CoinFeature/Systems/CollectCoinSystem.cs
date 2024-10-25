using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CoinFeature.Utils;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Extensions;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Utils;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Systems
{
    public class CollectCoinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CoinAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Exc] public readonly EcsTagPool<CollectedMarker> CollectedMarker;

            [Inc] public readonly EcsPool<RendererRef> Renderers;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<Coins> Coins;
            [Opt] public readonly EcsTagPool<CoinCollectedEvent> CollectedEvent;

            [Opt] public readonly EcsPool<TargetEntity> Target;

            [Opt] public readonly EcsTagPool<DeleteEntityRequest> DeleteEntity;
            [Opt] public readonly EcsTagPool<DestroyViewRequest> DestroyView;
            [Opt] public readonly EcsTagPool<CoinCollectAnimationCompletedEvent> CoinCollectAnimationCompletedEvent;
        }

        private class MovingAnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovingMarker))]
            [IncImplicit(typeof(AnimalTag))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
            [Inc] public readonly EcsPool<WorldPosition> WorldPositions;
            [Inc] public readonly EcsPool<MovementDirection> MovementDirections;
            [Inc] public readonly EcsPool<BoundExtents> BoundExtents;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int coin in _world.Where(out CoinAspect coinAspect))
            {
                foreach (int animal in _world.Where(out MovingAnimalAspect movingAnimalAspect))
                {
                    if (!movingAnimalAspect.ActiveGameFields.Read(animal).Value.TryGetID(out int gameFieldID))
                        continue;

                    ref readonly WorldPosition worldPosition = ref movingAnimalAspect.WorldPositions.Read(animal);

                    ref readonly BoundExtents boundExtents = ref movingAnimalAspect.BoundExtents.Read(animal);

                    GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                    ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(gameFieldID);

                    ref readonly MovementDirection movementDirection =
                        ref movingAnimalAspect.MovementDirections.Read(animal);

                    int2 animalCellPosition = GetCellPosition(
                        worldPosition.Value,
                        movementDirection.Value,
                        boundExtents.Value,
                        in gameField);

                    ref readonly CellPosition coinCellPosition = ref coinAspect.CellPositions.Read(coin);

                    if (math.all(coinCellPosition.Value == animalCellPosition))
                    {
                        coinAspect.CollectedMarker.Add(coin);

                        ref GameObjectConnect gameObjectConnect = ref coinAspect.GameObjectConnects.Get(coin);

                        Animate(coin, ref gameObjectConnect, coinAspect);

                        foreach (int player in _world.Where(out PlayerAspect _))
                        {
                            CoinUtils.Update(
                                coinable: player,
                                coins: coinAspect.Coins.Read(coin).Value);

                            coinAspect.CollectedEvent.Add(coin);
                        }
                    }
                }
            }
        }

        private static int2 GetCellPosition(float3 position, int2 direction, float3 boundExtents,
            in GameField gameField)
        {
            GridUtils.Grid grid = gameField.ToGrid();

            return GridUtils.GetCellPosition(
                worldPosition: position + direction.xyy * boundExtents,
                grid);
        }

        private void Animate(int coin, ref GameObjectConnect goConnect, CoinAspect coinAspect)
        {
            Sequence.Create()
                .Group(
                    Tween.Position(
                        target: goConnect.Connect.transform,
                        endValue: new Vector3(
                            goConnect.Connect.transform.position.x,
                            9,
                            goConnect.Connect.transform.position.y),
                        duration: 0.8f,
                        ease: Ease.OutQuint))
                .Group(
                    Tween.MaterialColor(
                        target: coinAspect.Renderers.Read(coin).Value.material,
                        endValue: Color.clear,
                        duration: 0.8f,
                        ease: Ease.OutQuint))
                .ChainCallback(
                    target: goConnect.Connect,
                    static connect =>
                    {
                        if (!connect.Entity.TryGetID(out _))
                            return;

                        EcsWorld world = connect.World;

                        int catcher = world.NewEntity();

                        CoinCatcherAspect.CoinCollectAnimationCompletedCatcher catcherAspect =
                            world.GetAspect<CoinCatcherAspect.CoinCollectAnimationCompletedCatcher>();

                        catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = connect.Entity;
                        catcherAspect.CatchCoinCollectAnimationCompletedRequest.Add(catcher);
                    });
        }
    }
}