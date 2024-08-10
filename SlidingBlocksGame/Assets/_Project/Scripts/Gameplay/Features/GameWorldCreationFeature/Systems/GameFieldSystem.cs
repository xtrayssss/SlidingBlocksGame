using System;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GameFieldSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class GenerationAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GenerateGameFieldRequest))]
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Opt] public readonly EcsTagPool<GameFieldGeneratedEvent> GameFieldGenerated;
        }

        private class DestructionAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestructionGameFieldRequest))]
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Opt] public readonly EcsTagPool<GameFieldDestructedEvent> GameFieldDestructed;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GenerationAspect aspect))
                Generate(aspect, entity);

            foreach (int entity in _world.Where(out DestructionAspect aspect))
                Destruct(aspect, entity);
        }

        private void Generate(GenerationAspect aspect, int entity)
        {
            ref GameField gameField = ref aspect.GameFields.Get(entity);

#if UNITY_EDITOR
            GameObject container = new GameObject(name: "GameField");
#endif

            int counter = 0;

            for (int x = 0; x < gameField.Size; x++)
            {
                for (int z = 0; z < gameField.Size; z++)
                {
                    if (x >= gameField.EdgeSize && x < gameField.EdgeSize + gameField.CenterSize ||
                        z >= gameField.EdgeSize && z < gameField.EdgeSize + gameField.CenterSize)
                    {
                        float3 position = new Vector3(
                            x * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.x, 0,
                            z * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.z);

#if UNITY_EDITOR
                        GameObject cell = Object.Instantiate(
                            original: gameField.CellPrefab,
                            position: position,
                            rotation: Quaternion.identity,
                            parent: container.transform);

#else
                        GameObject cell = Object.Instantiate(
                            original: gameField.CellPrefab,
                            position: position,
                            rotation: Quaternion.identity);
#endif

                        gameField.Cells[counter++] = new GameField.Cell
                        {
                            View = cell,
                            CellPosition = new float2(x, z),
                            WorldPosition = position
                        };
                    }
                }
            }

            aspect.GameFieldGenerated.Add(entity);
        }

        private void Destruct(DestructionAspect aspect, int entity)
        {
            ref GameField gameField = ref aspect.GameFields.Get(entity);

            foreach (ref GameField.Cell cell in gameField.Cells.AsSpan())
                Object.Destroy(cell.View);

            aspect.GameFieldDestructed.Add(entity);
        }
    }
}