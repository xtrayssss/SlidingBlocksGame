using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class PlayWithSelectedAnimalSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class ButtonClickedAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<PlayButtonTag> PlayButtonTag;
            [Inc] public readonly EcsTagPool<ButtonClickedEvent> Clicked;
        }

        private class AnimalsShopWindowAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalsShopWindowTag))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }
        private class PurchasedAnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ScrollSnappedMarker))]
            [IncImplicit(typeof(PurchasedMarker))]
            [Inc] public readonly EcsPool<Purchase> Purchases;
        }
        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<SelectionAnimalID> SelectionAnimalIndices;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out ButtonClickedAspect _))
            {
                foreach (int window in _world.Where(out AnimalsShopWindowAspect animalsShopWindowAspect))
                {
                    //animalsShopWindowAspect.GameObjectConnects.Get(window).Connect.gameObject.SetActive(false);

                    foreach (int animal in _world.Where(out PurchasedAnimalAspect purchasedAnimalAspect))
                    {
                        foreach (int player in _world.Where(out PlayerAspect playerAspect))
                            playerAspect.SelectionAnimalIndices.Get(player).Value =
                                purchasedAnimalAspect.Purchases.Read(animal).ProductIndex;
                    }
                }
            }
        }
    }
}