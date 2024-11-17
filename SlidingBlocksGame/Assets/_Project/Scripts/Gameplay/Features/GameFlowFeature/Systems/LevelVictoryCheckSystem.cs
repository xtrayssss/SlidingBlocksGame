using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class LevelVictoryCheckSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(AnimalPositionedMarker))]
            [ExcImplicit(typeof(LevelDefeatMarker))]
            [Inc] public readonly EcsPool<GameField> GameFields;

            [Exc] public readonly EcsTagPool<LevelVictoryEvent> LevelVictoryEvent;
            [Exc] public readonly EcsTagPool<LevelVictoryMarker> LevelVictoryMarker;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> CellOccupancyMarker;
            [Inc] public readonly EcsTagPool<WithinCenterMarker> WithinCenterMarker;
        }

        private class GameOverTimerExpiredAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CooldownExpiredMarker> CooldownExpiredMarker;
            [Inc] public readonly EcsTagPool<GameOverTimerTag> GameOverTimerTag;
        }

        public void Run()
        {
            Debug.Log("UPDATED");
            
            foreach (int entity in _world.Where(out LevelAspect aspect))
            {
                ref readonly GameField gameField = ref aspect.GameFields.Read(entity);

                if (_world.Where(out AnimalAspect _).Count == gameField.EdgeSize * gameField.EdgeSize &&
                    _world.Where(out GameOverTimerExpiredAspect _).Count == 0)
                {
                    aspect.LevelVictoryEvent.Add(entity);
                    aspect.LevelVictoryMarker.Add(entity);
                }
            }
        }
    }
}