using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.Components
{
    [Serializable]
    public struct GameTitleTag : IEcsTagComponent
    {

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.VisualFeature.Components", sourceClassName: "GameTitleTag/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : TagComponentTemplate<GameTitleTag>
        {
        }
    }
}