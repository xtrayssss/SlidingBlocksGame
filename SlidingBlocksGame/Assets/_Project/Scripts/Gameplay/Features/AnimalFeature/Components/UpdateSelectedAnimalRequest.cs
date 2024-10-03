using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.Components
{
    public struct UpdateSelectedAnimalRequest : IEcsComponent
    {
        public ushort SelectedID;
    }
}