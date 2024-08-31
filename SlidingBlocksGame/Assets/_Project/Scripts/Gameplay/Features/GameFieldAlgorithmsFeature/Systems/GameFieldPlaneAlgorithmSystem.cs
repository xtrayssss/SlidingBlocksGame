using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Systems
{
    public class GameFieldPlaneAlgorithmSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class GenerationAlgorithmAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldPlaneAlgorithmTag))]
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }
        
        private class DestructionAlgorithmAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldPlaneAlgorithmTag))]
            [IncImplicit(typeof(GameFieldDestructRequest))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }
        
        private class TargetLevelAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
           
            [Opt] public readonly EcsTagPool<GameFieldGeneratedEvent> GameFieldGenerated;
            [Opt] public readonly EcsTagPool<GameFieldDestructedEvent> GameFieldDestructed;
            [Opt] public readonly EcsTagPool<GameFieldGeneratedPlaneAlgorithmMarker> GameFieldGeneratedPlaneAlgorithm;
        }
        
        public void Run()
        {
            foreach (int entity in _world.Where(out GenerationAlgorithmAspect aspect))
                Generate(
                    generationAspect: aspect,
                    algorithm: entity,
                    levelAspect: _world.GetAspect<TargetLevelAspect>());

            foreach (int entity in _world.Where(out DestructionAlgorithmAspect aspect))
                Destruct(
                    destructionAspect: aspect,
                    algorithm: entity, 
                    levelAspect: _world.GetAspect<TargetLevelAspect>());
        }

        private void Generate(GenerationAlgorithmAspect generationAspect, int algorithm, TargetLevelAspect levelAspect)
        {
            Debug.Log($"Generate {algorithm}");
            if (!generationAspect.Targets.Read(algorithm).Value.TryGetID(out int levelID) || !levelAspect.IsMatches(levelID))
                return;
            
            levelAspect.GameFieldGeneratedPlaneAlgorithm.Add(levelID);
            
            ref GameField gameField = ref levelAspect.GameFields.Get(levelID);

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
            
            levelAspect.GameFieldGenerated.Add(levelID);
            levelAspect.GameFieldGenerated.Add(algorithm);
        }

        private void Destruct(DestructionAlgorithmAspect destructionAspect, int algorithm, TargetLevelAspect levelAspect)
        {
            if (!destructionAspect.Targets.Read(algorithm).Value.TryGetID(out int levelID) || !levelAspect.IsMatches(levelID))
                return;
            
            ref GameField gameField = ref levelAspect.GameFields.Get(levelID);

            foreach (ref GameField.Cell cell in gameField.Cells.AsSpan())
                Object.Destroy(cell.View);

            levelAspect.GameFieldDestructed.Add(levelID);
            levelAspect.GameFieldDestructed.Add(algorithm);
        }
    }
}