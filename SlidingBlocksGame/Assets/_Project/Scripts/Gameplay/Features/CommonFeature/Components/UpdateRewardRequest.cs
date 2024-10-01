using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    public struct UpdateRewardRequest : IEcsComponent
    {
        public long Time;
        public int Count;
        public bool Overwrite;
    }
}