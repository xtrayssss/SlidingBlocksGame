using DCFApixels.DragonECS;

<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/Components/UpdateBestScoreRequest.cs
namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
========
namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameProgressFeature/Components/UpdateBestScoreRequest.cs
{
    public struct UpdateBestScoreRequest : IEcsComponent
    {
        public bool Overwrite;
        public int Value;
    }
}