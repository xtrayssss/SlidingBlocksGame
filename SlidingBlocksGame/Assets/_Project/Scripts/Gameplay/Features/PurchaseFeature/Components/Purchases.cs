using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.Components
{
    [Serializable]
    [MetaGroup("Purchase")]
    public struct Purchases : IEcsComponent
    {
        public EcsGroup Entities;
        public EcsEntityConnect[] Prefabs;

            [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.PurchaseFeature.Components",
            sourceClassName: "Purchases/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<Purchases>
        {
        }
    }
}