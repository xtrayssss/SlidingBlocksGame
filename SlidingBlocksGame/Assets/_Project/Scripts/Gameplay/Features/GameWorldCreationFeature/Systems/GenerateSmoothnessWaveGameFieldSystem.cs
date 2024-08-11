using System;
using System.Threading.Tasks;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GenerateSmoothnessWaveGameFieldSystem : IEcsRun
    {
        [EcsInject] private EcsWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<WaveSmoothnessAlgorithm> Waves;
            [Inc] public readonly EcsPool<GameField> Fields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                CreateFieldWithWaveEffect(aspect.Fields.Read(entity), aspect.Waves.Read(entity));
            }
        }

        async void CreateFieldWithWaveEffect(GameField field, WaveSmoothnessAlgorithm wave)
        {
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

                        GameObject tile = Object.Instantiate(field.CellPrefab, position, Quaternion.identity);

                        tile.transform.localScale = float3.zero;

                        GrowTile(tile, wave);

                        await Task.Delay(TimeSpan.FromSeconds(wave.GrowthDuration / (field.Size * field.Size)));
                    }
                }
            }
        }

        private async void GrowTile(GameObject tile, WaveSmoothnessAlgorithm wave)
        {
            float3 initialScale = tile.transform.localScale;
            float3 targetScale = new float3(1);

            float elapsedTime = 0;

            while (elapsedTime < wave.GrowthDuration)
            {
                elapsedTime += Time.deltaTime;
                tile.transform.localScale = math.lerp(initialScale, targetScale, elapsedTime / wave.GrowthDuration);

                await Task.Yield();
            }

            tile.transform.localScale = targetScale;
        }
    }
}