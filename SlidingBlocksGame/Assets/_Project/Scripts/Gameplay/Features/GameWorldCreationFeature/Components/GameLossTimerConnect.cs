using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct GameLossTimerConnect : IEcsComponent
    {
        public EcsEntityConnect Value;

        private sealed class Template : ComponentTemplate<GameLossTimerConnect>
        {
        }
    }
}