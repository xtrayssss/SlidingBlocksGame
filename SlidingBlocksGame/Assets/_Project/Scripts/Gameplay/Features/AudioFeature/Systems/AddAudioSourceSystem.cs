using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.AudioFeature.Systems
{
    public class AddAudioSourceSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AudioTypeRef> AudioTypes;
            [Exc] public readonly EcsPool<AudioSourceRef> AudioSources;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref AudioSourceRef audioSource = ref aspect.AudioSources.Add(entity);

                audioSource.Value = aspect.AudioTypes.Read(entity).GetAudioSource();
            }
        }
    }
}