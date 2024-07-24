using _Project.Scripts.Gameplay.Features.EasingFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CreateGameFieldSystem : IEcsInit
    {
        private readonly EcsEntityConnect _blockPrefab;
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> Fields;
        }

        public CreateGameFieldSystem(EcsEntityConnect blockPrefab) =>
            _blockPrefab = blockPrefab;

        public void Init()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref GameField field = ref aspect.Fields.Get(entity);

                GameObject container = new GameObject(name: "GameField");

                field.EdgeSize = field.Size / 3;
                field.CenterSize = field.Size - 2 * field.EdgeSize;

                entlong block = _world.NewEntityLong();
                EcsEntityConnect connect = Object.Instantiate(_blockPrefab);
                connect.ConnectWith(block, true);
                
                connect.transform.position = new Vector3(0, 0, -2);

                _world.GetPool<EasingDestination>().Get(block.ID).Original = new float3(0, 0, -2);
                _world.GetPool<EasingDestination>().Get(block.ID).Destination = new float3(10, 0, -2);

                for (int x = 0; x < field.Size; x++)
                {
                    for (int z = 0; z < field.Size; z++)
                    {
                        if (x >= field.EdgeSize && x < field.EdgeSize + field.CenterSize ||
                            z >= field.EdgeSize && z < field.EdgeSize + field.CenterSize)
                        {
                            Object.Instantiate(field.BlockPrefab, new Vector3(
                                    x * (field.CellSize + field.Offset) + field.OriginPosition.x, 0,
                                    z * (field.CellSize + field.Offset) + field.OriginPosition.z), Quaternion.identity,
                                container.transform);
                        }
                    }
                }
            }
        }
    }
}