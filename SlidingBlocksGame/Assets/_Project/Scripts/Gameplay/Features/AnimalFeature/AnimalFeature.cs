using _Project.Scripts.Gameplay.Features.AnimalFeature.Components;
using _Project.Scripts.Gameplay.Features.AnimalFeature.Systems;
using _Project.Scripts.Gameplay.Features.AudioFeature;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
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
                
                // audio feature
                .AddAudioSystem<AnimalSpawnedEvent, AnimalSpawnedAudioConfig>();
        }
    }
}