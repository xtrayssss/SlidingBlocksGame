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
    public class GameOverTimerFeature<TMask> : EcsModule<TMask> where TMask : EcsAspect, new()
    {
        protected override void Import(Builder builder)
        {
            builder
                .AutoDelTag<GameOverTimerCreatedEvent>()
                .AddSystem(new CreateGameOverTimerSystem())
                .AutoDelTag<CreateGameOverTimerRequest>()

                // ui feature
                .AutoDelTag<GameOverTimerOpenedEvent>()
                .AddSystem(new DisplayGameOverTimerSystem())
                .AddSystem(new DisplayTimerProgressSystem())
                //
                .AutoDelTag<GameOverTimerClosedEvent>()
                .AddSystem(new CatchGameOverTimerSystem())
                .AddSystem(new CloseGameOverTimerSystem())
                .AutoDelTag<CloseGameOverTimerRequest>()
                
                // audio feature
                .AddAudioSystem<CooldownTickEvent, GameOverTimerTickAudioConfig>();
        }
    }
}