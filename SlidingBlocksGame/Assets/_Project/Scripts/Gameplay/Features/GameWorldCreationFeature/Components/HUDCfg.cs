using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components
{
    [Serializable]
    public struct HUDCfg : IEcsComponent
    {
        public ScriptableEntityTemplate Value;

        private sealed class Template : ComponentTemplate<HUDCfg>
        {
        }
    }
}