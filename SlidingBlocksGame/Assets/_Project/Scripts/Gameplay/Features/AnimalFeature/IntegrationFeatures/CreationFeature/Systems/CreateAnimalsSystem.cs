using System;
using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.PlayerFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.CreationFeature.Systems
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
            [Opt] public readonly EcsPool<RendererRef> MeshRenderers;
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
    }
}