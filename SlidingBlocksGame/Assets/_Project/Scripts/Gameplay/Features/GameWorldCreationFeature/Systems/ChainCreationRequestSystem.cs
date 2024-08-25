using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class ChainCreationRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(CreationChainTag))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }        
        
        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Inc] public readonly EcsPool<SpawningAudioConfig> SpawningAudioConfigs;
        }
        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Opt] public readonly EcsTagPool<AnimalPositionedEvent> AnimalPositionedEvent;
            [Opt] public readonly EcsTagPool<AnimalPositionedMarker> AnimalPositionedMarker;
        }
        private class AudioAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsTagPool<PlayAudioRequest> PlayAudio;
            [Opt] public readonly EcsPool<AudioSourceRef> AudioSource;
        }
        
        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    AnimalAspect animalAspect = _world.GetAspect<AnimalAspect>();

                    if (!animalAspect.IsMatches(targetID)) 
                        continue;
                    
                    animalAspect.GameObjectConnects.Read(targetID).Connect.gameObject.SetActive(true);

                    int audio = _world.NewEntity(animalAspect.SpawningAudioConfigs.Read(targetID).Value);

                    AudioAspect audioAspect = _world.GetAspect<AudioAspect>();
                    
                    audioAspect.AudioSource.Add(audio).Value = AudioSingleton.Instance.SfxSource;
                    audioAspect.PlayAudio.Add(audio);
                }
            }
            
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    LevelAspect levelAspect = _world.GetAspect<LevelAspect>();
                    
                    if (levelAspect.IsMatches(targetID))
                    {
                        Debug.Log("Animal positioned");
                        levelAspect.AnimalPositionedEvent.Add(targetID);
                        levelAspect.AnimalPositionedMarker.Add(targetID);
                    }
                }
            }
        }
    }
}