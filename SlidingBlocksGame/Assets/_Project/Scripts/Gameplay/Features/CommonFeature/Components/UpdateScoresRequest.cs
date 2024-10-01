using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    public struct UpdateScoresRequest : IEcsComponent
    {
        public int Value;
        public bool Overwrite;
    }
}