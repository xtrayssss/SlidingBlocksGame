using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    public struct BestScoreUpdatedEvent : IEcsComponent
    {
        public int Delta;
    }
}