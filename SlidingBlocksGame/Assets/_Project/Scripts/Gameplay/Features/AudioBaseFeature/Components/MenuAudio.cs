using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioBaseFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct MenuAudio : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components", sourceClassName: "MenuAudio/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<MenuAudio>
        {
        }
    }
}