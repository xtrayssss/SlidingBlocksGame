using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Utils
{
    public static class CoinUtils
    {
        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateCoinsRequest> UpdateCoins;
            [Inc] public readonly EcsPool<TargetEntity> Coinable;
        }
        
        public static void Update(int coinable, int coins, bool overwrite = false)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            Aspect aspect = world.GetAspect<Aspect>();

            int request = world.NewEntity();
            
            ref UpdateCoinsRequest updateRequest = ref aspect.UpdateCoins.Add(request);
            updateRequest.Value = coins;
            updateRequest.Overwrite = overwrite;
            aspect.Coinable.Add(request).Value = coinable.ToEntityLong(world);
        }
    }
}