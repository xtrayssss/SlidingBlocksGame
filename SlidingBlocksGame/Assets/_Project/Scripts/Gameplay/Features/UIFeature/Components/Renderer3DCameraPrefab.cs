using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct Renderer3DCameraPrefab : IEcsComponent
    {
        public Camera Value;

        private sealed class Template : ComponentTemplate<Renderer3DCameraPrefab>
        {
        }
    }
}