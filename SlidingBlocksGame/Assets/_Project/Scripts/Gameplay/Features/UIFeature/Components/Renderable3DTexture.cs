using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    [MetaGroup("UI")]
    public struct Renderable3DTexture : IEcsComponent
    {
        public RenderTexture Value;

        private sealed class Template : ComponentTemplate<Renderable3DTexture>
        {
        }
    }
}