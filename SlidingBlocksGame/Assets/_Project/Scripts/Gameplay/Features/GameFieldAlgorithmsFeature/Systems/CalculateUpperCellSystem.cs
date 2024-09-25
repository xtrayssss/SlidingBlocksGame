using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Systems
{
    public class CalculateUpperCellSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref GameField gameField = ref aspect.GameFields.Get(entity);
                
                GameObject gameObject = Object.Instantiate(gameField.CellPrefab);

                gameObject.transform.localScale =
                    new Vector3(gameField.CellSize, gameField.CellSize, gameField.CellSize);

                MeshRenderer renderer = gameObject.GetComponentInChildren<MeshRenderer>();
                
                float lowerY = renderer.bounds.center.y - renderer.bounds.extents.y;
                float upperY = lowerY + renderer.bounds.size.y;

                gameField.CellUpper = upperY;
                
                Object.Destroy(gameObject);
            }
        }
    }
}