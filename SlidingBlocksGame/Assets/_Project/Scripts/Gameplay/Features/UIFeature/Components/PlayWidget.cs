using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct PlayWidget : IEcsComponent
    {
        public GameObject PlayButton;
        public GameObject ReplayButton;

        private sealed class Template : ComponentTemplate<PlayWidget>
        {
        }
    }
}