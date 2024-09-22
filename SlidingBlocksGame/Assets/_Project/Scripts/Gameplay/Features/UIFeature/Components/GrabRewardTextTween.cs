using System;
using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct GrabRewardTextTween : IEcsComponent
    {
        public Tween Value;

        private sealed class Template : ComponentTemplate<GrabRewardTextTween>
        {
        }
    }
}