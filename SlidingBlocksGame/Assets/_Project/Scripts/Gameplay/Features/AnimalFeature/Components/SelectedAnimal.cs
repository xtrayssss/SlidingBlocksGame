using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.Components
{
    [Serializable]
    [MetaGroup("Animal")]
    public struct SelectedAnimal : IEcsComponent
    {
        public EcsEntityConnect Prefab;
        public ushort ID;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components",
            sourceClassName: "SelectedAnimal/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<SelectedAnimal>
        {
        }
    }
}