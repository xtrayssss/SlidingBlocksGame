using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.CreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Systems;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.AnimalFeature.Systems;
using _Project.Scripts.Gameplay.Features.AudioBaseFeature;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature
{
    public class AnimalFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                // creation feature
                .AddUnique(new CreateAnimalsSystem())
                .AutoDelTag<AnimalPositionedEvent>()
                .AutoDelTag<AnimalSpawnedEvent>()
                .AddUnique(new AnimalCreationChainStrategySystem())
                .AutoDelTag<CreateAnimalsRequest>()

                // movement feature
                .AddUnique(new MovementAnimalsChainStrategySystem())

                // destruction feature
                .AutoDelTag<AnimalDestructedEvent>()
                .AddUnique(new AnimalDestructionChainStrategySystem())
                
                // selection feature
                .AutoDelEntityTag<SelectedAnimalUpdatedEvent>()
                .AddUnique(new UpdateSelectedAnimalSystem())
                .AutoDelEntityComponent<UpdateSelectedAnimalRequest>()
                
                // audio feature
                .AddAudioSystem<AnimalSpawnedEvent, AnimalSpawnedAudioConfig>();
        }
    }
}