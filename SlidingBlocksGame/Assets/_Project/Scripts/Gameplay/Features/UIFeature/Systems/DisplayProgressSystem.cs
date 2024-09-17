using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class DisplayProgressSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ScoreUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScoreTag))]
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> Texts;

            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
        }

        private class CoinsUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
            [Inc] public readonly EcsPool<CoinsUpdatedEvent> CoinsUpdatedEvent;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<Coins> Coins;
            [Opt] public readonly EcsPool<Scores> Scores;
        }

        private class ScoreUpdatedEventAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
            [Inc] public readonly EcsPool<ScoresUpdatedEvent> ScoresUpdatedEvent;
        }

        private class CoinUIAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CoinUITag))]
            [Inc] public readonly EcsPool<TextMeshProUGUIRef> Texts;

            [Inc] public readonly EcsPool<RectTransformRef> RectTransforms;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Coins> Coins;
            [Inc] public readonly EcsPool<Scores> Scores;
        }

        private class GameCreatedAspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<GameCreatedEvent> _gameCreatedEvents;
        }

        public void Run()
        {
            foreach (int @event in _world.Where(out CoinsUpdatedEventAspect coinsUpdatedEventAspect))
            {
                foreach (int entity in _world.Where(out CoinUIAspect coinUIAspect))
                {
                    if (!coinsUpdatedEventAspect.TargetEntities.Read(@event).Value.TryGetID(out int targetID))
                        continue;

                    TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                    coinUIAspect.Texts.Get(entity).Value.text =
                        targetEntityAspect.Coins.Read(targetID).Value.ToString();

                    if (coinsUpdatedEventAspect.CoinsUpdatedEvent.Read(@event).Delta != 0)
                    {
                        Tween.PunchScale(
                            target: coinUIAspect.RectTransforms.Read(entity).Value,
                            strength: new Vector3(0.5f, 0.5f),
                            duration: 0.2f);
                    }
                }
            }

            foreach (int @event in _world.Where(out ScoreUpdatedEventAspect scoreUpdatedEventAspect))
            {
                foreach (int entity in _world.Where(out ScoreUIAspect aspect))
                {
                    if (!scoreUpdatedEventAspect.TargetEntities.Read(@event).Value.TryGetID(out int targetID))
                        continue;

                    TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                    aspect.Texts.Get(entity).Value.text = targetEntityAspect.Scores.Get(targetID).Value.ToString();

                    if (scoreUpdatedEventAspect.ScoresUpdatedEvent.Read(@event).Delta != 0)
                    {
                        Tween.PunchScale(
                            target: aspect.RectTransforms.Read(entity).Value,
                            strength: new Vector3(0.5f, 0.5f),
                            duration: 0.2f);
                    }
                }
            }

            // primary
            foreach (int _ in _world.Where(out GameCreatedAspect _))
            {
                foreach (int entity in _world.Where(out ScoreUIAspect aspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    {
                        aspect.Texts.Get(entity).Value.text = playerAspect.Scores.Get(player).Value.ToString();

                        Tween.PunchScale(aspect.RectTransforms.Read(entity).Value, new Vector3(0.5f, 0.5f), 0.2f);
                    }
                }

                foreach (int entity in _world.Where(out CoinUIAspect coinUIAspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    {
                        coinUIAspect.Texts.Get(entity).Value.text = playerAspect.Coins.Read(player).Value.ToString();

                        Tween.PunchScale(coinUIAspect.RectTransforms.Read(entity).Value, new Vector3(0.5f, 0.5f), 0.2f);
                    }
                }
            }
        }
    }
}