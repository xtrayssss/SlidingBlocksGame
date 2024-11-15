using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.Utils
{
    public static class UIUtils
    {
        public static readonly Vector2 WOBBLE_OFFSET = new Vector2(0, 45);

        public static TweenSettings<Vector3> WobbleLoopingSettings = new TweenSettings<Vector3>
        {
            settings = new TweenSettings
            {
                cycles = -1,
                cycleMode = CycleMode.Yoyo,
                ease = Ease.Linear,
                duration =  0.6f
            }
        };

        public static TweenSettings<Vector3> WobbleSettings = new TweenSettings<Vector3>
        {
            settings = new TweenSettings
            {
                ease = Ease.Linear,
                duration =  0.6f
            }
        };
    }
}