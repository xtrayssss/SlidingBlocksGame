using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.ScrollSnapFeature.Components
{
    [Serializable]
    [MetaGroup("ScrollSnap")]
    public struct ScaleEffect : IEcsComponent
    {
        public Vector2 SelectedItemScale;
        public Vector2 UnselectedItemScale;

        public sealed class Template : ComponentTemplate<ScaleEffect>
        {
        }
    }
}