using System;
using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct OpenCloseTween : IEcsComponent
    {
        public Sequence Value;

        private sealed class Template : ComponentTemplate<OpenCloseTween>
        {
        }
    }
}