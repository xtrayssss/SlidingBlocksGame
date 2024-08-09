using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class ChainCreationRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredMarker))]
            [IncImplicit(typeof(CreationChainTag))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }        
        
        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [Inc] public readonly EcsPool<GameObjectConnect> GameObjectConnects;
        }
        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Opt] public readonly EcsTagPool<AnimalPositionedEvent> AnimalPositioned;
        }
        
        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    AnimalAspect animalAspect = _world.GetAspect<AnimalAspect>();
                    
                    if (animalAspect.IsMatches(targetID)) 
                        animalAspect.GameObjectConnects.Read(targetID).Connect.gameObject.SetActive(true);
                }
            }
            
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int targetID))
                {
                    LevelAspect levelAspect = _world.GetAspect<LevelAspect>();
                    
                    if (levelAspect.IsMatches(targetID)) 
                        levelAspect.AnimalPositioned.Add(targetID);
                }
            }
        }
    }
}