using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems
{
    public class UpdatePurchaseSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class ClearPurchasesRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<ClearPurchasesRequest> ClearPurchasesRequest;
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<PurchasesClearedEvent> PurchasesClearedEvent;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        private class PurchasedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PurchaseTag))]
            [Inc] public readonly EcsTagPool<PurchasedMarker> PurchasedMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out ClearPurchasesRequestAspect aspect))
            {
                ref readonly TargetEntity targetEntity = ref aspect.TargetEntities.Read(entity);

                if (!targetEntity.Value.TryGetID(out _))
                    continue;

                foreach (int purchase in _world.Where(out PurchasedAspect purchasedAspect))
                    purchasedAspect.PurchasedMarker.Del(purchase);

                TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                int @event = _world.NewEntity();
                targetEntityAspect.PurchasesClearedEvent.Add(@event);
                targetEntityAspect.TargetEntity.Add(@event).Value = targetEntity.Value;
            }
        }
    }
}