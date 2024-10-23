using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Components
{
    public struct UpdateCoinsRequest : IEcsComponent    
    {
        public int Value;
        public bool Overwrite;
    }
}