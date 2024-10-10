using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct GameScreen : IEcsComponent
    {
        public EcsEntityConnect SettingsPopupConnect;
        public EcsEntityConnect AnimalsShopWindowConnect;
        public EcsEntityConnect RewardWidgetConnect;
        public EcsEntityConnect GameTileWidgetConnect;
        [FormerlySerializedAs("GameLossTimerWidgetConnect")] public EcsEntityConnect GameOverTimerWidgetConnect;
        public EcsEntityConnect PlayWidgetConnect;
        public EcsEntityConnect ScoreWidgetConnect;
        public EcsEntityConnect CoinsWidgetConnect;
        public EcsEntityConnect BestScoreWidgetConnect;
        public EcsEntityConnect TutorialWindowConnect;
        public Canvas Canvas;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "GameScreen/Template", sourceAssembly: "Assembly-CSharp")]

        private sealed class Template : ComponentTemplate<GameScreen>
        {
        }
    }
}