using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct TapToExitWidget : IEcsComponent
    {
        public Button ExitButton;

        private sealed class Template : ComponentTemplate<TapToExitWidget>
        {
        }
    }
}