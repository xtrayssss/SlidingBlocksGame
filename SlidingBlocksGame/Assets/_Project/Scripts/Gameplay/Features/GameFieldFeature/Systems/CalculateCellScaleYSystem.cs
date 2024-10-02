using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Systems
{
    public class CalculateCellScaleYSystem : IEcsRun
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

                gameField.CellScaleY = gameField.BaseCellScaleY *
                                              ((float)gameField.BaseSize / gameField.Size);
            }
        }
    }
}