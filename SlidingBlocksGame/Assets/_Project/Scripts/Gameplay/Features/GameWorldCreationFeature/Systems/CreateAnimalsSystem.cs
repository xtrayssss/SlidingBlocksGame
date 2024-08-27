using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
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
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly GameField gameField = ref aspect.Fields.Read(entity);

                Span<GameField.AnimalsData> animals = new Span<GameField.AnimalsData>(gameField.Animals);

                AnimalAspect animalAspect = _world.GetAspect<AnimalAspect>();

                _world.NewEntityLong(aspect.StrategyConfigs.Read(entity).Value);

                foreach (ref GameField.AnimalsData animalData in animals)
                {
                    entlong animal = _world.NewEntityLong();

                    EcsEntityConnect view = Object.Instantiate(
                        original: gameField.AnimalPrefab,
                        position: animalData.Position + new float3(gameField.UnitCellTopOffset),
                        rotation: animalData.Rotation);

                    animalData.View = view;

                    view.ConnectWith(animal, applyTemplates: true);

                    ScaleRenderer(animal);

                    _world.GetPool<CellPosition>().Add(animal.ID).Value = animalData.CellPosition;
                    
                    animalAspect.Direction.Add(animal.ID).Value = animalData.InvertedSide;

                    animalAspect.ActiveGameField.Add(animal.ID).Value = entity.ToEntityLong(_world);
                }
            }
        }

        private void ScaleRenderer(entlong animal)
        {
            ref RendererRef renderer = ref _world.GetPool<RendererRef>().Get(animal.ID);

            var meshFilter = renderer.Value.GetComponent<MeshFilter>();

            float size = 1 / meshFilter.mesh.bounds.size.x;

            renderer.Value.transform.localScale = new Vector3(size, size, size);
        }
    }
}