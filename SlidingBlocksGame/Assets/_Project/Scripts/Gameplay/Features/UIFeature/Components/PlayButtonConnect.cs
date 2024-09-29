using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct PlayButtonConnect :  IEcsComponent
    {
        public EcsEntityConnect Play;
        public EcsEntityConnect Replay;

        private sealed class Template : ComponentTemplate<PlayButtonConnect>
        {
        }
    }
}