using DCFApixels.DragonECS;

<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/Components/BestScoreUpdatedEvent.cs
namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
========
namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameProgressFeature/Components/BestScoreUpdatedEvent.cs
{
    public struct BestScoreUpdatedEvent : IEcsComponent
    {
        public int Delta;
    }
}