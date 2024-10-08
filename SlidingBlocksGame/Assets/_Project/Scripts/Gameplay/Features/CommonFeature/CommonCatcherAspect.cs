using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature
{
    public class CommonCatcherAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
    }
}