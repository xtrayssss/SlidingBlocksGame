using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Components
{
    [Serializable]
    public struct CanvasRef : IEcsComponent
    {
        public Canvas Value;

        private sealed class Template : ComponentTemplate<CanvasRef>
        {
        }
    }
}