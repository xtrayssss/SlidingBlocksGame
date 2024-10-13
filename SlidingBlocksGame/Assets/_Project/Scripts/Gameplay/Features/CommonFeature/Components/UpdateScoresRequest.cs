using DCFApixels.DragonECS;

<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/CommonFeature/Components/UpdateScoresRequest.cs
namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
========
namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.Components
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/GameProgressFeature/Components/UpdateScoresRequest.cs
{
    public struct UpdateScoresRequest : IEcsComponent
    {
        public int Value;
        public bool Overwrite;
    }
}