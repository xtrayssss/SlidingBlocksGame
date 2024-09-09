using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct SelectedAnimal : IEcsComponent
    {
        public EcsEntityConnect Prefab;
        public ushort ID;
        
        private sealed class Template : ComponentTemplate<SelectedAnimal>
        {
        }
    }
}