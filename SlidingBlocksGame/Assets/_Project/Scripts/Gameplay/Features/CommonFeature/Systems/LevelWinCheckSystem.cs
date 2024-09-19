using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class LevelWinCheckSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(AnimalPositionedMarker))]
            [ExcImplicit(typeof(LevelLostMarker))]
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Exc] public readonly EcsTagPool<LevelWonEvent> LevelWonEvent;
            [Exc] public readonly EcsTagPool<LevelWonMarker> LevelWonMarker;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<CellOccupancyMarker> _cellOccupancyMarkers;
            [Inc] private readonly EcsTagPool<WithinCenterMarker> _withinCenterMarkers;
        }

        private class GameLossTimerExpiredAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CooldownExpiredMarker> CooldownExpiredMarker;
            [Inc] public readonly EcsTagPool<GameLossTimerTag> GameLossTimerTag;
        }


        public void Run()
        {
            foreach (int entity in _world.Where(out LevelAspect aspect))
            {
                ref readonly GameField gameField = ref aspect.GameFields.Read(entity);

                if (_world.Where(out AnimalAspect _).Count == gameField.EdgeSize * gameField.EdgeSize &&
                    _world.Where(out GameLossTimerExpiredAspect _).Count == 0)
                {
                    aspect.LevelWonEvent.Add(entity);
                    aspect.LevelWonMarker.Add(entity);
                }
            }
        }
    }
}