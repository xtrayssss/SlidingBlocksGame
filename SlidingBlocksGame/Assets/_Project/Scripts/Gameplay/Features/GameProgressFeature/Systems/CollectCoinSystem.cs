using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Systems
{
    public class CollectCoinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CoinAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Exc] public readonly EcsTagPool<CollectedMarker> CollectedMarker;

            [Inc] public readonly EcsPool<MeshRendererRef> MeshRenderers;
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
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Inc] public readonly EcsPool<MovementDirection> MovementDirections;
            [Inc] public readonly EcsPool<BoundExtents> BoundExtents;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        public void Run()
        {
            foreach (int coin in _world.Where(out CoinAspect coinAspect))
            {
                foreach (int animal in _world.Where(out MovingAnimalAspect movingAnimalAspect))
                {
                    if (!movingAnimalAspect.ActiveGameFields.Read(animal).Value.TryGetID(out int gameFieldID))
                        continue;

                    ref readonly GameObjectConnect goConnect = ref movingAnimalAspect.GameObjectConnects.Read(animal);

                    float3 transformPosition = goConnect.Connect.transform.position;

                    ref readonly BoundExtents boundExtents = ref movingAnimalAspect.BoundExtents.Read(animal);

                    ref readonly GameField gameField = ref _world.GetPool<GameField>().Read(gameFieldID);

                    ref readonly MovementDirection movementDirection =
                        ref movingAnimalAspect.MovementDirections.Read(animal);

                    int2 position = GridUtils.GetCellPosition(
                        worldPosition: transformPosition +
                                       movementDirection.Value.xyy *
                                       boundExtents.Value,
                        gameField: in gameField);

                    if (math.all(coinAspect.CellPositions.Read(coin).Value == position))
                    {
                        coinAspect.CollectedMarker.Add(coin);

                        ref GameObjectConnect gameObjectConnect = ref coinAspect.GameObjectConnects.Get(coin);

                        Animate(coin, coinAspect, ref gameObjectConnect);

                        foreach (int player in _world.Where(out PlayerAspect _))
                        {
                            ProgressUtils.UpdateCoins(player, coinAspect.Coins.Read(coin).Value);
                            coinAspect.CollectedEvent.Add(coin);
                        }
                    }
                }
            }
        }

        private void Animate(int coin, CoinAspect coinAspect, ref GameObjectConnect gameObjectConnect)
        {
            Sequence.Create()
                .Group(
                    Tween.Position(
                        target: gameObjectConnect.Connect.transform,
                        endValue: new Vector3(
                            gameObjectConnect.Connect.transform.position.x,
                            9,
                            gameObjectConnect.Connect.transform.position.y),
                        duration: 0.8f,
                        ease: Ease.OutQuint))
                .Group(
                    Tween.MaterialColor(
                        target: coinAspect.MeshRenderers.Read(coin).Value.material,
                        endValue: Color.clear,
                        duration: 0.8f,
                        ease: Ease.OutQuint))
                .ChainCallback(
                    target: gameObjectConnect.Connect,
                    static connect =>
                    {
                        if (!connect.Entity.TryGetID(out _))
                            return;

                        EcsWorld world = connect.Entity.World;

                        int catcher = world.NewEntity();

                        CoinCatcherAspect.CoinCollectAnimationCompletedCatcher catcherAspect =
                            world.GetAspect<CoinCatcherAspect.CoinCollectAnimationCompletedCatcher>();

                        catcherAspect.CommonCatcherAspect.TargetEntities.Add(catcher).Value = connect.Entity;
                        catcherAspect.CatchCoinCollectAnimationCompletedRequest.Add(catcher);
                    });
        }
    }
}