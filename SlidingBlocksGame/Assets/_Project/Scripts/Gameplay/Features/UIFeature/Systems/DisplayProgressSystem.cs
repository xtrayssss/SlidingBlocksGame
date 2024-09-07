using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using DCFApixels.DragonECS.RunnersCore;
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
            foreach (int _ in _world.Where(out CoinCollectedAspect _))
            {
                foreach (int entity in _world.Where(out CoinUIAspect coinUIAspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    {
                        coinUIAspect.Texts.Get(entity).Value.text = playerAspect.Coins.Read(player).Value.ToString();

                        Tween.PunchScale(coinUIAspect.RectTransforms.Read(entity).Value, new Vector3(0.5f, 0.5f), 0.2f);
                    }
                }
            }

            foreach (int _ in _world.Where(out ScoreUpdatedAspect _))
            {
                foreach (int entity in _world.Where(out ScoreUIAspect aspect))
                {
                    foreach (int player in _world.Where(out PlayerAspect playerAspect))
                    {
                        aspect.Texts.Get(entity).Value.text = playerAspect.Scores.Get(player).Value.ToString();

                        Tween.PunchScale(aspect.RectTransforms.Read(entity).Value, new Vector3(0.5f, 0.5f), 0.2f);
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