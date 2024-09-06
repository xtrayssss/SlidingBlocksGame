using System;
using DCFApixels.DragonECS;
using UnityEngine.UI;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct ButtonRef : IEcsComponent
    {
        public Button Value;

        private sealed class Template : ComponentTemplate<ButtonRef>
        {
        }
    }
}