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
                ref GameField gameField = ref aspect.Fields.Get(entity);

                GameObject container = new GameObject(name: "GameField");

                int counter = 0;
                
                for (int x = 0; x < gameField.Size; x++)
                {
                    for (int z = 0; z < gameField.Size; z++)
                    {
                        if (x >= gameField.EdgeSize && x < gameField.EdgeSize + gameField.CenterSize ||
                            z >= gameField.EdgeSize && z < gameField.EdgeSize + gameField.CenterSize)
                        {
                            GameObject cell = Object.Instantiate(gameField.TilePrefab,  new Vector3(
                                    x * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.x, 0,
                                    z * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.z),
                                Quaternion.identity,
                                container.transform);
                            
                            gameField.Cells[counter++] = new GameField.Cell
                            {
                                View = cell
                            };
                        }
                    }
                }
            }
        }
    }
}