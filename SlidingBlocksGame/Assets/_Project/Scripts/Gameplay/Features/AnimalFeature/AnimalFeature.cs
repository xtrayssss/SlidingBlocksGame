using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.CreationFeature.Systems;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.DestructionFeature.Systems;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.MovementFeature.Systems;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AnimalFeature
{
    public class AnimalFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                // creation feature
                .AddSystem(new CreateAnimalsSystem())
                .AutoDelTag<AnimalPositionedEvent>()
                .AutoDelTag<AnimalSpawnedEvent>()
                .AddSystem(new AnimalCreationChainStrategySystem())
                .AutoDelTag<CreateAnimalsRequest>()

                // movement feature
                .AddSystem(new MovementAnimalsChainStrategySystem())

                // destruction feature
                .AutoDelTag<AnimalDestructedEvent>()
                .AddSystem(new AnimalDestructionChainStrategySystem())
                .AddSystem(new DestroyAnimalsSystem())
                .AddSystem(new AnimalDeathSystem())
                
                // ui feature
                .AddSystem(new DisplayAnimalsShopWindowSystem())
                .AutoDelTag<AnimalPurchaseWindowClosedEvent>()
                .AddSystem(new CloseAnimalsShopWindowSystem())
                
                // audio feature
                .AddAudioSystem<AnimalSpawnedEvent, AnimalSpawnedAudioConfig>()
                .AddAudioSystem<DiedEvent, AnimalDeathAudioConfig>();
        }
    }
}