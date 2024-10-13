using _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Systems
{
    public class SetupScrollSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ScrollAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<ScrollSnap> ScrollSnap;
            [Inc] public readonly EcsPool<SetupScrollRequest> SetupScrollRequest;

            [Opt] public readonly EcsPool<ScrollToTargetState> ScrollToTargetState;
        }

        private class ScrollItemAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
            [Opt] public readonly EcsPool<ScrollItem> ScrollItems;
        }

        public void Run()
        {
            foreach (int scroll in _world.Where(out ScrollAspect scrollAspect))
            {
                ref SetupScrollRequest setupScrollRequest = ref scrollAspect.SetupScrollRequest.Get(scroll);

                ref ScrollSnap scrollSnap = ref scrollAspect.ScrollSnap.Get(scroll);
                scrollSnap.ItemCount = setupScrollRequest.Items.Count;
                scrollSnap.Positions = new float[scrollSnap.ItemCount];
                scrollSnap.Distance = CalculateDistance(in scrollSnap);
                scrollSnap.Items = setupScrollRequest.Items;

                ScrollItemAspect scrollItemAspect = _world.GetAspect<ScrollItemAspect>();

                for (int index = 0; index < setupScrollRequest.Items.Count; index++)
                {
                    int itemID = setupScrollRequest.Items[index];

                    ref GameObjectConnect itemConnect = ref scrollItemAspect.GoConnects.Get(itemID);
                    scrollSnap.Positions[index] = scrollSnap.Distance * index;
                    ref ScrollItem scrollItem = ref scrollItemAspect.ScrollItems.Add(itemID);
                    scrollItem.RectTransform = itemConnect.Connect.transform as RectTransform;
                    scrollItem.Graphic = itemConnect.Connect.GetComponentInChildren<Graphic>();
                    scrollItem.Index = index;
                }

                scrollSnap.TargetIndex = setupScrollRequest.ScrollToIndex;
                scrollAspect.ScrollToTargetState.Add(scroll);
            }
        }

        private static float CalculateDistance(in ScrollSnap scrollSnap) =>
            scrollSnap.ItemCount > 1 ? 1f / (scrollSnap.ItemCount - 1f) : 1;
    }
}