using System;
using DCFApixels.DragonECS;
using PrimeTween;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameScreenFeature.Components
{
    [Serializable]
    [MetaGroup("GameScreen")]
    public struct GameTitleWidget : IEcsComponent
    {
        public Tween WobbleTween;

        [MovedFrom(autoUpdateAPI: false,
            sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Components",
            sourceClassName: "GameTitleWidget/Template", sourceAssembly: "UIFeature.Components")]
        private sealed class Template : ComponentTemplate<GameTitleWidget>
        {
        }
    }
}