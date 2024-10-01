using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.Systems
{
    public class CreateAnimalsSystem : IEcsRun
    {
        [EcsInject] private EcsWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(CreateAnimalsRequest))]
            [Inc] public readonly EcsPool<CreationAnimalStrategyCfg> CreationStrategyConfigs;

            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<MovementDirection> Direction;
            [Opt] public readonly EcsPool<ActiveGameField> ActiveGameField;
            [Opt] public readonly EcsPool<BoundExtents> BoundsExtents;
            [Opt] public readonly EcsPool<MeshRendererRef> MeshRenderers;
        }

        private class SideAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<MovementDirection> MovementDirection;
            [Opt] public readonly EcsTagPool<SideTag> SideTag;
            [Opt] public readonly EcsPool<SideAnimals> SideAnimals;
            [Opt] public readonly EcsTagPool<LevelLifeTimeMarker> LevelLifeTime;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<SelectedAnimal> SelectedAnimalPrefabs;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LevelAspect aspect))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    ref readonly GameField gameField = ref aspect.GameFields.Read(level);

                    Span<GameField.AnimalsData> animals = new Span<GameField.AnimalsData>(gameField.Animals);

                    AnimalAspect animalAspect = _world.GetAspect<AnimalAspect>();

                    CreateCreationStrategy(aspect, level);

                    SideAspect sideAspect = _world.GetAspect<SideAspect>();

                    (int2 direction, EcsGroup animals) left = CreateSide(sideAspect, GridUtils.Left);
                    (int2 direction, EcsGroup animals) right = CreateSide(sideAspect, GridUtils.Right);
                    (int2 direction, EcsGroup animals) up = CreateSide(sideAspect, GridUtils.Up);
                    (int2 direction, EcsGroup animals) down = CreateSide(sideAspect, GridUtils.Down);

                    foreach (ref GameField.AnimalsData animalData in animals)
                    {
                        entlong animal = _world.NewEntityLong();

                        float3 cellUpperOffset = new float3(0, gameField.CellScaleY + gameField.UnitCellTopOffset, 0);
                        
                        EcsEntityConnect view = Object.Instantiate(
                            original: playerAspect.SelectedAnimalPrefabs.Read(player).Prefab,
                            position: animalData.Position + cellUpperOffset,
                            rotation: animalData.Rotation);

                        view.ConnectWith(animal, applyTemplates: true);

                        view.transform.localScale = animalData.Scale;

                        _world.GetPool<CellPosition>().Add(animal.ID).Value = animalData.CellPosition;

                        ref MovementDirection movementDirection = ref animalAspect.Direction.Add(animal.ID);

                        movementDirection.Value = animalData.InvertedSide;

                        animalAspect.ActiveGameField.Add(animal.ID).Value = level.ToEntityLong(_world);

                        animalAspect.BoundsExtents.Add(animal.ID).Value =
                            animalAspect.MeshRenderers.Read(animal.ID).Value.bounds.extents;

                        _world.GetPool<CalculateMovementSpeedRequest>().Add(animal.ID);

                        if (math.all(movementDirection.Value == right.direction))
                            right.animals.Add(animal.ID);
                        if (math.all(movementDirection.Value == left.direction))
                            left.animals.Add(animal.ID);
                        if (math.all(movementDirection.Value == up.direction))
                            up.animals.Add(animal.ID);
                        if (math.all(movementDirection.Value == down.direction))
                            down.animals.Add(animal.ID);
                    }
                }
            }
        }

        private void CreateCreationStrategy(LevelAspect levelAspect, int level)
        {
            ref readonly CreationAnimalStrategyCfg creationAnimalStrategyCfg = ref levelAspect.CreationStrategyConfigs.Read(level);
            entlong strategy = _world.NewEntityLong(creationAnimalStrategyCfg.Value);
            
            _world.GetPool<ApplyCreationStrategyRequest>().Add(strategy.ID);
        }

        private (int2 direction, EcsGroup animals) CreateSide(SideAspect sideAspect, int2 direction)
        {
            int side = _world.NewEntity();
            sideAspect.SideTag.Add(side);
            sideAspect.LevelLifeTime.Add(side);

            return (sideAspect.MovementDirection.Add(side).Value = direction,
                sideAspect.SideAnimals.Add(side).Value = EcsGroup.New(_world));
        }
    }
}