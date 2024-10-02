using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CollectFeature.Systems
{
    public class CollectCoinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CoinAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinTag))]
            [Inc] public readonly EcsPool<CellPosition> CellPositions;

            [Inc] public readonly EcsPool<MeshRendererRef> MeshRenderers;
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;

            [Inc] public readonly EcsPool<Coins> Coins;
            [Opt] public readonly EcsTagPool<CoinCollectedEvent> CollectedEvent;

            [Opt] public readonly EcsPool<TargetEntity> Target;

            [Opt] public readonly EcsTagPool<DeleteEntityRequest> DeleteEntity;
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
                    callback: target =>
                    {
                        if (!target.Entity.TryGetID(out int id))
                            return;

                        coinAspect.DeleteEntity.Add(id);
                        coinAspect.DestroyView.Add(id);
                    });
        }
    }
}