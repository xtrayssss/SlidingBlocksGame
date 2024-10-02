using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.PlayerFeature.Components
{
    [Serializable]
    public struct PlayerTag : IEcsTagComponent
    {
        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.CommonFeature.Components", sourceClassName: "PlayerTag/Template", sourceAssembly: "Assembly-CSharp")]

        private sealed class Template : TagComponentTemplate<PlayerTag>
        {
        }
    }
}