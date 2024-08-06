using System;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct RendererRef : IEcsComponent
    {
        public GameObject Value;

        private class Template : ComponentTemplate<RendererRef>
        {
            
        }
    }
}