using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.InputFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class LevelWinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class WinStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelWonEvent))]
                [Inc] public readonly EcsPool<GameFieldAlgorithms> GameFieldAlgorithmConfigs;

                [Opt] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
            }
        }

        private class AnimalDestructedStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelWonMarker))]
                [IncImplicit(typeof(AnimalDestructedEvent))]
                [Opt] public readonly EcsTagPool<GameFieldDestructRequest> GameFieldDestruct;
            }
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameTag))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLeveRequest;
        }

        private class GameLossTimerClosedStateAspect
        {
            public class OnUpdate : EcsAspectAuto
            {
                [Inc] public readonly EcsTagPool<GameLossTimerTag> GameLossTimerTag;
                [Inc] public readonly EcsTagPool<ClosedMarker> ClosedMarker;
            }
        }

        private class LevelClearedStateAspect
        {
            public class OnEnter : EcsAspectAuto
            {
                [IncImplicit(typeof(LevelTag))]
                [IncImplicit(typeof(LevelWonMarker))]
                [IncImplicit(typeof(LevelClearedEvent))]
                [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevel;
            }
        }

        private class GameLossTimerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameLossTimerTag))]
            [Opt] public readonly EcsTagPool<CloseGameLossTimerRequest> Close;
            [Opt] public readonly EcsTagPool<CooldownLockMarker> CooldownLockMarker;
        }

        private class DestructedGameFieldStateAspect : EcsAspectAuto
        {
            public class OnUpdate : EcsAspectAuto
            {
                [ExcImplicit(typeof(LevelClearedEvent))]
                [Inc] public readonly EcsTagPool<LevelTag> LevelTag;

                [Inc] public readonly EcsTagPool<GameFieldDestructedMarker> GameFieldDestructedMarker;
                [Inc] public readonly EcsTagPool<LevelWonMarker> LevelWonMarker;
            }
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Exc] public readonly EcsTagPool<LockGameInputMarker> LockGameInputMarker;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out WinStateAspect.OnEnter aspect))
            {
                Debug.Log("WinStateAspect.OnEnter");

                entlong strategy =
                    _world.NewEntityLong(aspect.DestructionAnimalStrategyConfigs.Read(level).Value);

                _world.GetPool<TargetEntity>().Add(strategy.ID).Value = _world.GetEntityLong(level);

                _world.GetPool<ApplyDestructionStrategyRequest>().Add(strategy.ID);

                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    playerAspect.LockGameInputMarker.Add(player);
                
                foreach (int timer in _world.Where(out GameLossTimerAspect gameLossTimerAspect))
                    gameLossTimerAspect.CooldownLockMarker.Add(timer);
            }

            foreach (int level in _world.Where(out AnimalDestructedStateAspect.OnEnter levelAspect))
            {
                levelAspect.GameFieldDestruct.Add(level);

                foreach (int timer in _world.Where(out GameLossTimerAspect gameLossTimerAspect))
                    gameLossTimerAspect.Close.Add(timer);
            }

            foreach (int level in _world.Where(out DestructedGameFieldStateAspect.OnUpdate _))
            {
                foreach (int _ in _world.Where(out GameLossTimerClosedStateAspect.OnUpdate _))
                    _world.GetPool<CleanupLevelRequest>().Add(level);
            }

            foreach (int level in _world.Where(out LevelClearedStateAspect.OnEnter aspect))
            {
                foreach (int game in _world.Where(out GameAspect _))
                    aspect.NextLevel.Add(game);

                _world.DelEntity(level);
            }
        }
    }
}