using DCFApixels.DragonECS;

<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/Components/UpdateCoinsRequest.cs
namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
========
namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameProgressFeature/Components/UpdateCoinsRequest.cs
{
    public struct UpdateCoinsRequest : IEcsComponent    
    {
        public int Value;
        public bool Overwrite;
    }
}