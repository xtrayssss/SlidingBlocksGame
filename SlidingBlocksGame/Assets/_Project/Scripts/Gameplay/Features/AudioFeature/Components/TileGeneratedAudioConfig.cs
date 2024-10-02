using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("Audio")]
    public struct TileGeneratedAudioConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

		[MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioFeature.Components", sourceClassName: "TileGeneratedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]


		private sealed class Template : ComponentTemplate<TileGeneratedAudioConfig>
        {
        }
    }
}