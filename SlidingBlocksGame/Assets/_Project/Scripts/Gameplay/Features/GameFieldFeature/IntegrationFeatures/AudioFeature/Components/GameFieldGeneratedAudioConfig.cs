using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.IntegrationFeatures.AudioFeature.Components
{
    [Serializable]
    [MetaGroup("GameField/Audio")]
    public struct GameFieldGeneratedAudioConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        [MovedFrom(autoUpdateAPI: false, sourceNamespace: "_Project.Scripts.Gameplay.Features.AudioBaseFeature.Components",
            sourceClassName: "GameFieldGeneratedAudioConfig/Template", sourceAssembly: "Assembly-CSharp")]
        private sealed class Template : ComponentTemplate<GameFieldGeneratedAudioConfig>
        {
        }
    }
}