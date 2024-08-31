using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class PlayAudioSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayAudioRequest))]
            [Inc] public readonly EcsPool<Audio> Audios;

            [Inc] public readonly EcsPool<AudioSourceRef> AudioSources;
        }

        private class LoopAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [Inc] public readonly EcsPool<Audio> Audios;

            [Inc] public readonly EcsPool<AudioSourceRef> AudioSources;
            [Opt] public readonly EcsTagPool<RefreshCooldownRequest> Refresh;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out LoopAspect aspect))
            {
                aspect.AudioSources.Read(entity).Value.PlayOneShot(aspect.Audios.Read(entity).Value);

                aspect.Refresh.Add(entity);
            }

            foreach (int entity in _world.Where(out Aspect aspect))
                aspect.AudioSources.Read(entity).Value.PlayOneShot(aspect.Audios.Read(entity).Value);
        }
    }
}