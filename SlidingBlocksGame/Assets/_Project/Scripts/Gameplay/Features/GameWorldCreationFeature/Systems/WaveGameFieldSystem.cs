using System.Collections;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class WaveGameFieldSystem : IEcsRun
    {
        [EcsInject] private EcsWorld _world;

        private class GenerationAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GenerateWaveGameFieldRequest))]
            [Inc] public readonly EcsPool<WaveAlgorithm> Waves;

            [Inc] public readonly EcsPool<GameField> GameFields;

            [Opt] public readonly EcsTagPool<GameFieldGeneratedEvent> GameFieldGenerated;
        }

        private class DestructionAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestructionGameFieldRequest))]
            [Inc] public readonly EcsPool<WaveAlgorithm> Waves;

            [Inc] public readonly EcsPool<GameField> GameFields;

            [Opt] public readonly EcsTagPool<GameFieldDestructedEvent> GameFieldDestructed;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out GenerationAspect aspect))
            {
                Object.FindAnyObjectByType<MonoBehaviour>()
                    .StartCoroutine(
                        Generate(
                            gameField: aspect.GameFields.Read(entity),
                            wave: aspect.Waves.Read(entity),
                            generationAspect: aspect,
                            entity: entity));
            }

            foreach (int entity in _world.Where(out DestructionAspect aspect))
            {
                Object.FindAnyObjectByType<MonoBehaviour>()
                    .StartCoroutine(
                        Destruct(
                            gameField: aspect.GameFields.Read(entity),
                            wave: aspect.Waves.Read(entity),
                            destructionAspect: aspect,
                            entity: entity));
            }
        }

        private IEnumerator Generate(GameField gameField, WaveAlgorithm wave, GenerationAspect generationAspect, int entity)
        {
            float3 waveOrigin = new float3(gameField.Size / 2f * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.x,
                0, gameField.Size / 2f * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.z);

            int counter = 0;
            
            for (int x = 0; x < gameField.Size; x++)
            {
                for (int z = 0; z < gameField.Size; z++)
                {
                    if (x >= gameField.EdgeSize && x < gameField.EdgeSize + gameField.CenterSize ||
                        z >= gameField.EdgeSize && z < gameField.EdgeSize + gameField.CenterSize)
                    {
                        float3 position = new float3(
                            x * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.x, 0,
                            z * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.z);

                        float delay = math.distance(position, waveOrigin) * wave.Speed;

                        yield return new WaitForSeconds(delay);

                        GameObject view = Object.Instantiate(gameField.CellPrefab, position, Quaternion.identity);

                        generationAspect.GameFields.Get(entity).Cells[counter++] = new GameField.Cell
                        {
                            View = view,
                            CellPosition = new float2(x, z),
                            WorldPosition = position
                        };
                    }
                }
            }

            generationAspect.GameFieldGenerated.Add(entity);
        }

        private IEnumerator Destruct(GameField gameField, WaveAlgorithm wave, DestructionAspect destructionAspect,
            int entity)
        {
            float3 waveOrigin = new float3(gameField.Size / 2f * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.x,
                0, gameField.Size / 2f * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.z);

            foreach (GameField.Cell cell in destructionAspect.GameFields.Get(entity).Cells)
            {
                float3 position = new float3(
                    cell.CellPosition.x * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.x, 0,
                    cell.CellPosition.y * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.z);
                
                float delay = math.distance(position, waveOrigin) * wave.Speed;

                yield return new WaitForSeconds(delay);

                Object.Destroy(cell.View);
            }

            destructionAspect.GameFieldDestructed.Add(entity);
        }
    }
}