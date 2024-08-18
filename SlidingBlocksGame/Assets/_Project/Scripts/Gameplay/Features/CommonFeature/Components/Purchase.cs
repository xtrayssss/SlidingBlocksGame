using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct Purchase : IEcsComponent
    {
        public GameObjectConnect Prefab;
        public int Price;

        private sealed class Template : ComponentTemplate<Purchase>
        {
        }
    }
}