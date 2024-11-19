using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature.IntegrationFeatures.UIFeature.Systems
{
    public class DisplayTimerProgressSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CooldownInterval> CooldownIntervals;
            [Inc] public readonly EcsPool<GameOverTimerWidget> GameOverTimerWidgets;
            [Inc] public readonly EcsPool<Cooldown> Cooldowns;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                aspect.GameOverTimerWidgets.Get(entity).Fill.fillAmount =
                    aspect.CooldownIntervals.Get(entity).Elapsed / aspect.Cooldowns.Read(entity).Duration;
            }
        }
    }
}