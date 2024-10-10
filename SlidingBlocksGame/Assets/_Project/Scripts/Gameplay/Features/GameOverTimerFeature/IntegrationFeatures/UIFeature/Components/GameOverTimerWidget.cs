using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components
{
    [Serializable]
    [MetaGroup("GameOverTimer/UI")]
    public struct GameOverTimerWidget : IEcsComponent
    {
        public Image Fill;

        private sealed class Template : ComponentTemplate<GameOverTimerWidget>
        {
        }
    }
}