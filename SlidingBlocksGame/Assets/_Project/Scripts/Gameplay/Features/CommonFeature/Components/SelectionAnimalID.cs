using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    [Serializable]
    public struct SelectionAnimalID : IEcsComponent
    {
        public uint Value;

        private sealed class Template : ComponentTemplate<SelectionAnimalID>
        {
        }
    }
}