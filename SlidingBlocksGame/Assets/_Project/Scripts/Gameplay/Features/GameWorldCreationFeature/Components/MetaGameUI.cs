using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct MetaGameUI : IEcsComponent
    {
        public GameObject[] Value;

        private sealed class Template : ComponentTemplate<MetaGameUI>
        {
        }
    }
}