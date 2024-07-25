using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class GenerateGameFieldSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GenerateGameFieldRequest))] [Inc]
            public readonly EcsPool<GameField> Fields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref GameField field = ref aspect.Fields.Get(entity);

                GameObject container = new GameObject(name: "GameField");

                for (int x = 0; x < field.Size; x++)
                {
                    for (int z = 0; z < field.Size; z++)
                    {
                        if (x >= field.EdgeSize && x < field.EdgeSize + field.CenterSize ||
                            z >= field.EdgeSize && z < field.EdgeSize + field.CenterSize)
                        {
                            Object.Instantiate(field.TilePrefab, new Vector3(
                                    x * (field.CellSize + field.Offset) + field.OriginPosition.x, 0,
                                    z * (field.CellSize + field.Offset) + field.OriginPosition.z),
                                Quaternion.identity,
                                container.transform);
                        }
                    }
                }
            }
        }
    }
}