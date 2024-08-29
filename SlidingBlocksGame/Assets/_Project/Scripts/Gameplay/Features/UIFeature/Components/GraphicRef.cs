using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct GraphicRef : IEcsComponent
    {
        public Graphic Value;

        private sealed class Template : ComponentTemplate<GraphicRef>
        {
        }
    }
}