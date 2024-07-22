using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateGameFieldSystem : IEcsInit
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> Fields;
        }

        public void Init()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly GameField field = ref aspect.Fields.Read(entity);

                GameObject container = new GameObject(name: "GameField");

                int edgeSize = field.size / 3;
                int centerSize = field.size - 2 * edgeSize;

                for (int x = 0; x < field.size; x++)
                {
                    for (int z = 0; z < field.size; z++)
                    {
                        if (x >= edgeSize && x < edgeSize + centerSize || z >= edgeSize && z < edgeSize + centerSize)
                        {
                            Object.Instantiate(field.blockPrefab, new Vector3(
                                    x * (field.cellSize + field.offset) + field.originPosition.x, 0,
                                    z * (field.cellSize + field.offset) + field.originPosition.z), Quaternion.identity,
                                container.transform);
                        }
                    }
                }
            }
        }
    }
}