using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    public struct UpdateCoinsRequest : IEcsComponent    
    {
        public int Value;
        public bool Overwrite;
    }
}