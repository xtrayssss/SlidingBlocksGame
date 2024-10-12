using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    public struct ScrollItem : IEcsComponent
    {
        public RectTransform RectTransform;
        public int Index;
        public Graphic Graphic;
    }
}