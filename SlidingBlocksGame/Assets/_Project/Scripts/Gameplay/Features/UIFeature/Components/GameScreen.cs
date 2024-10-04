using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct GameScreen : IEcsComponent
    {
        public EcsEntityConnect SettingsPopupConnect;
        public EcsEntityConnect AnimalsShopWindowConnect;
        public EcsEntityConnect RewardWidgetConnect;
        public EcsEntityConnect GameTileWidgetConnect;
        public EcsEntityConnect GameLossTimerWidgetConnect;
        public EcsEntityConnect PlayWidgetConnect;
        public EcsEntityConnect ScoreWidgetConnect;
        public EcsEntityConnect CoinsWidgetConnect;
        public EcsEntityConnect BestScoreWidgetConnect;
        public Canvas Canvas;

        private sealed class Template : ComponentTemplate<GameScreen>
        {
        }
    }
}