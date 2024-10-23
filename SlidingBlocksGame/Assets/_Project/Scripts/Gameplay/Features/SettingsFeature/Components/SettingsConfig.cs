using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.SettingsFeature.Components
{
    [Serializable]
    [MetaGroup("Settings")]
    public struct SettingsConfig : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private sealed class Template : ComponentTemplate<SettingsConfig>
        {
        }
    }
}