using _Project.Scripts.Gameplay.Features.SettingsFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.SettingsFeature.Systems
{
    public class CreateSettingsSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class CreateSettingsRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CreateSettingsRequest> CreateSettingsRequest;
            [Inc] public readonly EcsPool<SettingsConfig> SettingsConfig;
        }

        private class SettingsAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<SettingsCreatedEvent> SettingsCreatedEvent;
        }

        public void Run()
        {
            foreach (int request in _world.Where(out CreateSettingsRequestAspect requestAspect))
            {
                ref readonly SettingsConfig settingsConfig = ref requestAspect.SettingsConfig.Read(request);
                int settings = _world.NewEntity(settingsConfig.Value);

                SettingsAspect settingsAspect = _world.GetAspect<SettingsAspect>();
                settingsAspect.SettingsCreatedEvent.Add(settings);
            }
        }
    }
}