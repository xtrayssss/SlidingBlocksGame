using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class CalculateMovementSpeedSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CalculateMovementSpeedRequest))]
            [Inc] public readonly EcsPool<MovementSpeedFactor> MovementSpeedFactors;

            [Inc] public readonly EcsPool<ActiveGameField> ActiveGameFields;
        }

        private class GameFieldAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref var factor = ref aspect.MovementSpeedFactors.Get(entity);

                if (!aspect.ActiveGameFields.Read(entity).Value.TryGetID(out int gameFieldID))
                    continue;

                GameFieldAspect gameFieldAspect = _world.GetAspect<GameFieldAspect>();

                ref readonly GameField gameField = ref gameFieldAspect.GameFields.Read(gameFieldID);

                factor.Value = factor.Base * ((float)gameField.Size / gameField.BaseSize);
            }
        }
    }
}