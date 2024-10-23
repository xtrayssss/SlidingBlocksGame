using _Project.Scripts.Gameplay.Features.CoinFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CoinFeature.Systems
{
    public class UpdateCoinsSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateCoinsRequest> UpdateCoinsRequest;
            [Inc] public readonly EcsPool<TargetEntity> Coinables;
        }

        private class CoinableAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Coins> Coins;
        }
        
        private class EventAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<CoinsUpdatedEvent> CoinsUpdatedEvent;
            [Opt] public readonly EcsPool<TargetEntity> Coinable;
        }

        public void Run()
        {
            foreach (int request in _world.Where(out RequestAspect aspect))
            {
                ref readonly TargetEntity coinable = ref aspect.Coinables.Read(request);

                if (!coinable.Value.TryGetID(out int coinableID))
                    continue;

                ref readonly UpdateCoinsRequest updateRequest = ref aspect.UpdateCoinsRequest.Read(request);

                CoinableAspect coinableAspect = _world.GetAspect<CoinableAspect>();

                ref Coins coins = ref coinableAspect.Coins.Get(coinableID);

                coins.Value += updateRequest.Value;

                GenerateEvent(coinable);
                
                _world.DelEntity(request);
            }
        }

        private void GenerateEvent(TargetEntity targetEntity)
        {
            EventAspect eventAspect = _world.GetAspect<EventAspect>();
            int entity = _world.NewEntity();
            eventAspect.CoinsUpdatedEvent.Add(entity);
            eventAspect.Coinable.Add(entity).Value = targetEntity.Value;
        }
    }
}