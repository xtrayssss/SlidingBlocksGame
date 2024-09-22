using System;
using System.Collections;
using DCFApixels.DragonECS;
using PrimeTween;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct OpenCloseSequence : IEcsComponent
    {
        public Sequence Value;

        private sealed class Template : ComponentTemplate<OpenCloseSequence>
        {
        }
    }
}