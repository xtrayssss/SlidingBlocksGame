using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateAnimalsSystem : IEcsRun
    {
        [EcsInject] private EcsWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CreateAnimalsRequest))]
            [Inc] public readonly EcsPool<CreationAnimalStrategyCfg> StrategyConfigs;

            [Inc] public readonly EcsPool<GameField> Fields;
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
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    ref readonly GameField gameField = ref aspect.Fields.Read(entity);

                    Span<GameField.AnimalsData> animals = new Span<GameField.AnimalsData>(gameField.Animals);

                    AnimalAspect animalAspect = _world.GetAspect<AnimalAspect>();

                    _world.NewEntityLong(aspect.StrategyConfigs.Read(entity).Value);

                    SideAspect sideAspect = _world.GetAspect<SideAspect>();

                    var left = CreateSide(sideAspect, GridUtils.Left);
                    var right = CreateSide(sideAspect, GridUtils.Right);
                    var up = CreateSide(sideAspect, GridUtils.Up);
                    var down = CreateSide(sideAspect, GridUtils.Down);

                    foreach (ref GameField.AnimalsData animalData in animals)
                    {
                        entlong animal = _world.NewEntityLong();

                        EcsEntityConnect view = Object.Instantiate(
                            original: playerAspect.SelectedAnimalPrefabs.Read(player).Prefab,
                            position: animalData.Position,
                            rotation: animalData.Rotation);

                        animalData.View = view;

                        view.ConnectWith(animal, applyTemplates: true);

                        view.transform.localScale = animalData.Scale;

                        _world.GetPool<CellPosition>().Add(animal.ID).Value = animalData.CellPosition;

                        ref MovementDirection movementDirection = ref animalAspect.Direction.Add(animal.ID);

                        movementDirection.Value = animalData.InvertedSide;

                        animalAspect.ActiveGameField.Add(animal.ID).Value = entity.ToEntityLong(_world);

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