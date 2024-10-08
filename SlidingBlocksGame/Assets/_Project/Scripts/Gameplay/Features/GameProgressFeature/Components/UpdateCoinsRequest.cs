using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
{
    public struct UpdateCoinsRequest : IEcsComponent    
    {
        public int Value;
        public bool Overwrite;
    }
}