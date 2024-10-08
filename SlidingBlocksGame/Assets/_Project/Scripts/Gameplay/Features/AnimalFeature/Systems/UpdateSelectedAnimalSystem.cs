using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature.Systems
{
    public class UpdateSelectedAnimalSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class UpdateSelectedAnimalRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<UpdateSelectedAnimalRequest> UpdatePurchasesRequest;
            [Inc] public readonly EcsPool<TargetEntity> TargetEntities;
        }

        private class TargetEntityAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<SelectedAnimal> SelectedAnimals;
            [Inc] public readonly EcsPool<AnimalPrefabs> AnimalPrefabs;

            [Opt] public readonly EcsTagPool<SelectedAnimalUpdatedEvent> SelectedAnimalUpdatedEvent;
            [Opt] public readonly EcsPool<TargetEntity> TargetEntity;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out UpdateSelectedAnimalRequestAspect aspect))
            {
                ref readonly TargetEntity targetEntity = ref aspect.TargetEntities.Read(entity);

                if (!targetEntity.Value.TryGetID(out int targetID))
                    continue;

                ref readonly UpdateSelectedAnimalRequest updateSelectedAnimalRequest =
                    ref aspect.UpdatePurchasesRequest.Read(entity);

                TargetEntityAspect targetEntityAspect = _world.GetAspect<TargetEntityAspect>();

                ref SelectedAnimal selectedAnimal = ref targetEntityAspect.SelectedAnimals.Get(targetID);

                selectedAnimal.ID = updateSelectedAnimalRequest.SelectedID;
                selectedAnimal.Prefab = targetEntityAspect.AnimalPrefabs.Read(targetID).Animals[selectedAnimal.ID];

                int @event = _world.NewEntity();
                targetEntityAspect.SelectedAnimalUpdatedEvent.Add(@event);
                targetEntityAspect.TargetEntity.Add(@event).Value = targetEntity.Value;
            }
        }
    }
}