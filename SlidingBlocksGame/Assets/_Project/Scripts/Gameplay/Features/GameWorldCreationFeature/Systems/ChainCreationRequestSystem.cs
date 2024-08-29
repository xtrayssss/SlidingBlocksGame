using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class ChainCreationRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;
        
        private class CooldownCompletedAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(CreationChainTag))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }        
        
        private class TargetAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
            [Opt] public readonly EcsTagPool<SpawnedEvent> Spawned;
        }
        
        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Opt] public readonly EcsTagPool<AnimalPositionedEvent> AnimalPositionedEvent;
            [Opt] public readonly EcsTagPool<AnimalPositionedMarker> AnimalPositionedMarker;
        }
        
        public void Run()
        {
            foreach (int entity in _world.Where(out CooldownCompletedAspect cooldownCompletedAspect))
            {
                if (cooldownCompletedAspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    TargetAspect targetAspect = _world.GetAspect<TargetAspect>();

                    if (!targetAspect.IsMatches(targetID)) 
                        continue;
                    
                    targetAspect.GameObjectConnects.Read(targetID).Connect.gameObject.SetActive(true);
                    targetAspect.Spawned.Add(targetID);
                }
            }
            
            foreach (int entity in _world.Where(out CooldownCompletedAspect aspect))
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