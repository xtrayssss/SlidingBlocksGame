using _Project.Scripts.Gameplay.Features.AudioFeature.Extensions;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Systems;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature
{
    public class GameOverTimerFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
            builder
                .AutoDelTag<GameOverTimerCreatedEvent>()
                .AddUnique(new CreateGameOverTimerSystem())
                .AutoDelTag<CreateGameOverTimerRequest>()

                // ui feature
                .AutoDelTag<GameOverTimerOpenedEvent>()
                .AddUnique(new DisplayGameOverTimerSystem())
                .AddUnique(new DisplayTimerProgressSystem())
                //
                .AutoDelTag<GameOverTimerClosedEvent>()
                .AddUnique(new CatchGameOverTimerSystem())
                .AddUnique(new CloseGameOverTimerSystem())
                .AutoDelTag<CloseGameOverTimerRequest>()
                
                // audio feature
                .AddAudioSystem<CooldownTickEvent, GameOverTimerTickAudioConfig>();
        }
    }
}