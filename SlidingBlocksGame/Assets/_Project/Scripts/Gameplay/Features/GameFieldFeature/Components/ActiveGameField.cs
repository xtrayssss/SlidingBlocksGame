using System;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.GameFieldFeature.Components
{
    [Serializable]
    public struct ActiveGameField : IEcsComponent
    {
        public entlong Value;
    }
}