using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CollectFeature
{
    public class UpdatePlayerProgressSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<Coins> Coins;

            [Inc] public readonly EcsPool<Scores> Scores;
        }

        private class CoinCollectedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinCollectedEvent))]
            [Inc] public readonly EcsPool<Coins> Coins;
        }

        private class ScoreUpdatedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [IncImplicit(typeof(SpawnedEvent))]
            [Inc] public readonly EcsPool<Scores> Scores;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out CoinCollectedAspect coinCollectedAspect))
            {
                foreach (int entity in _world.Where(out PlayerAspect playerAspect))
                    playerAspect.Coins.Get(entity).Value += coinCollectedAspect.Coins.Read(@event).Value;
            }

            foreach (int @event in _world.Where(out ScoreUpdatedAspect scoreUpdateAspect))
            {
                foreach (int entity in _world.Where(out PlayerAspect playerAspect))
                    playerAspect.Scores.Get(entity).Value += scoreUpdateAspect.Scores.Read(@event).Value;
            }
        }
    }
}