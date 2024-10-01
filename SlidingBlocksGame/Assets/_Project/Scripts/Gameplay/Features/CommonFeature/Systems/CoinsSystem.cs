using _Project.Scripts.Gameplay.Features.CollectFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Utils;
using DCFApixels.DragonECS;
using UnityEngine.Scripting.APIUpdating;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class CoinsSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateCoinsRequest> UpdateCoinsRequest;
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Coins> Coins;

            [Opt] public readonly EcsPool<CoinsUpdatedEvent> CoinsUpdatedEvent;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out RequestAspect aspect))
            {
                ref readonly TargetEntity targetEntity = ref aspect.TargetEntities.Read(entity);

                if (!targetEntity.Value.TryGetID(out int targetID))
                    continue;

                ref readonly UpdateCoinsRequest updateCoinsRequest = ref aspect.UpdateCoinsRequest.Read(entity);

                TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                ref Coins coins = ref targetEntityAspect.Coins.Get(targetID);

                int lastCoins = coins.Value;

                if (updateCoinsRequest.Overwrite)
                    coins.Value = updateCoinsRequest.Value;
                else
                    coins.Value += updateCoinsRequest.Value;

                int @event = _world.NewEntity();
                targetEntityAspect.CoinsUpdatedEvent.Add(@event).Delta = coins.Value - lastCoins;
                targetEntityAspect.TargetEntity.Add(@event).Value = targetEntity.Value;
            }
        }
    }
}