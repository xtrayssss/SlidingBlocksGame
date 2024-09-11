using System;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Components
{
    [Serializable]
    public struct ResetProgressButtonTag : IEcsTagComponent
    {
        public class Template : TagComponentTemplate<ResetProgressButtonTag>
        {
        }
    }
}