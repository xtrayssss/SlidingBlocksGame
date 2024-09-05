using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct MeshRendererRef : IEcsComponent
    {
        public MeshRenderer Value;

        private sealed class Template : ComponentTemplate<MeshRendererRef>
        {
        }
    }
}