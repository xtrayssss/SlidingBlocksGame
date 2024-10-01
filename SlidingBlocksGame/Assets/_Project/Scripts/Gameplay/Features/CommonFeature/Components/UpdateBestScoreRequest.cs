using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    public struct UpdateBestScoreRequest : IEcsComponent
    {
        public bool Overwrite;
        public int Value;
    }
}