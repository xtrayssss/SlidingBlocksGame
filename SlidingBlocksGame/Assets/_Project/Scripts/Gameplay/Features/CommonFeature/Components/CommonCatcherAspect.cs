using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Components
{
    public class CommonCatcherAspect : EcsAspectAuto
    {
        [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
    }
}