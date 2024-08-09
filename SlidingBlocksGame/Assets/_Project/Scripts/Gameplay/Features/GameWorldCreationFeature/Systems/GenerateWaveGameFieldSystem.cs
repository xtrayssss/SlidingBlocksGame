using System.Collections;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GenerateWaveGameFieldSystem : IEcsRun
    {
        [EcsInject] private EcsWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GenerateWaveGameFieldRequest))]
            [Inc] public readonly EcsPool<WaveAlgorithm> Waves;

            [Inc] public readonly EcsPool<GameField> Fields;

            [Opt] public readonly EcsTagPool<GameFieldGeneratedEvent> GameFieldGenerated;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Object.FindAnyObjectByType<MonoBehaviour>()
                    .StartCoroutine(
                        CreateFieldWithWaveEffect(
                            field: aspect.Fields.Read(entity),
                            wave: aspect.Waves.Read(entity),
                            aspect: aspect,
                            entity: entity));
            }
        }

        private IEnumerator CreateFieldWithWaveEffect(GameField field, WaveAlgorithm wave, Aspect aspect, int entity)
        {
            float3 waveOrigin = new float3(field.Size / 2f * (field.CellSize + field.Offset) + field.OriginPosition.x,
                0, field.Size / 2f * (field.CellSize + field.Offset) + field.OriginPosition.z);
            Debug.Log("123");

            for (int x = 0; x < field.Size; x++)
            {
                for (int z = 0; z < field.Size; z++)
                {
                    if (x >= field.EdgeSize && x < field.EdgeSize + field.CenterSize ||
                        z >= field.EdgeSize && z < field.EdgeSize + field.CenterSize)
                    {
                        float3 position = new float3(
                            x * (field.CellSize + field.Offset) + field.OriginPosition.x, 0,
                            z * (field.CellSize + field.Offset) + field.OriginPosition.z);

                        float delay = math.distance(position, waveOrigin) * wave.Speed;

                        yield return new WaitForSeconds(delay);

                        Object.Instantiate(field.CellPrefab, position, Quaternion.identity);
                    }
                }
            }

            Debug.Log("123");

            aspect.GameFieldGenerated.Add(entity);
        }
    }
}