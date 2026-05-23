using System;
using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.GameScreenFeature.Components
{
    [MovedFrom(autoUpdateAPI: false,
        sourceNamespace: "_Project.Scripts.Gameplay.Features.UIFeature.Components",
        sourceClassName: "UnlockButtonTag/Template", sourceAssembly: "Assembly-CSharp")]

    [Serializable]
    struct UnlockButtonTag : IEcsTagComponent
    {
        private sealed class Template : TagComponentTemplate<UnlockButtonTag>
        {
        }
    }

    [Serializable]
    [MetaGroup("GameScreen")]
    public struct GameScreen : IEcsComponent
    {
        public EcsEntityConnect SettingsPopupConnect;
        public EcsEntityConnect AnimalsShopWindowConnect;
        public EcsEntityConnect RewardWidgetConnect;
        public EcsEntityConnect GameTileWidgetConnect;
        public EcsEntityConnect GameOverTimerWidgetConnect;
        public EcsEntityConnect PlayWidgetConnect;
        public EcsEntityConnect ScoreWidgetConnect;
        public EcsEntityConnect CoinsWidgetConnect;
        public EcsEntityConnect BestScoreWidgetConnect;
        public EcsEntityConnect TutorialWindowConnect;
        public Canvas Canvas;
        public GraphicRaycaster GraphicRaycaster;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "GameScreen/Template", sourceAssembly: "UIFeature.Components")]
        private sealed class Template : ComponentTemplate<GameScreen>
        {
        }
    }
}