using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.MovementFeature
{
    public class CommonCatcherAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
    }
}