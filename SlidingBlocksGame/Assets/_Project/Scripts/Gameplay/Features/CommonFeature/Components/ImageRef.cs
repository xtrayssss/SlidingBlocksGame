using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct ImageRef : IEcsComponent
    {
        public Image Value;

        private sealed class Template : ComponentTemplate<ImageRef>
        {
        }
    }
}