using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CollectFeature.Components
{
    public struct CoinsUpdatedEvent : IEcsComponent
    {
        public float Delta;
    }
}