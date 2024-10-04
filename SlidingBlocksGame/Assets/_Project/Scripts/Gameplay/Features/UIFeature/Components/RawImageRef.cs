using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct RawImageRef : IEcsComponent
    {
        public RawImage Value;

        private sealed class Template : ComponentTemplate<RawImageRef>
        {
        }
    }
}