using System.Linq;
using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class ScrollEffectRequestSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private static readonly EcsDefaultWorld WORLD = EcsDefaultWorldSingletonProvider.Instance.Get();

        private class ScrollAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScrollUnlockedMarker))]
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnaps;
        }

        private class EffectAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<ApplyEffectRequest> ApplyEffectRequest;
            [Opt] public readonly EcsPool<EffectDisplacement> Displacements;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out ScrollAspect scrollAspect))
            {
                ref ScrollSnap scrollSnap = ref scrollAspect.ScrollSnaps.Get(scroll);

                foreach (ScriptableEntityTemplate effectCfg in scrollSnap.Effects)
                {
                    for (var index = 0; index < scrollSnap.Items.Count; index++)
                    {
                        var item = scrollSnap.Items[index];
                        int effect = _world.NewEntity(effectCfg);

                        EffectAspect effectAspect = _world.GetAspect<EffectAspect>();
                        ref ApplyEffectRequest applyEffectRequest = ref effectAspect.ApplyEffectRequest.Add(effect);
                        ref readonly EffectDisplacement displacement = ref effectAspect.Displacements.Read(effect);
                        applyEffectRequest.Ratio = GetEffectRatio(GetEffectDisplacementBasedOnPos(
                            in scrollSnap,
                            scrollSnap.Positions[index],
                            displacement.EffectedDistanceBasedOnItemSize));

                        EcsGroup ecsGroup = EcsGroup.New(_world);
                        applyEffectRequest.Items = ecsGroup;
                        ecsGroup.Add(item);
                    }
                }
            }
        }

        private float GetEffectDisplacementBasedOnPos(in ScrollSnap scrollSnap, float pos, float effect)
        {
            var signedDist = (pos - scrollSnap.ScrollPosition) / (scrollSnap.Distance * effect);
            return Mathf.Clamp(signedDist, -1, 1);
        }

        private EcsGroup GetAliveItems(ScrollSnap scrollSnap)
        {
            EcsGroup aliveItems = EcsGroup.New(_world);

            foreach (int item in scrollSnap.Items.Where(static item => item.ToEntityLong(WORLD).IsAlive))
                aliveItems.Add(item);

            return aliveItems;
        }

        private float GetEffectRatio(float displacement) =>
            1 - Mathf.Abs(displacement);
    }
}