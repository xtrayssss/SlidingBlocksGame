using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.CreationFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CreationFeature.Systems
{
    public class CreationChainStrategySystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class CooldownExpiredAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredEvent))]
            [IncImplicit(typeof(CreationChainStrategyTag))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class CreatableAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<GameObjectConnect> GoConnects;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out CooldownExpiredAspect cooldownAspect))
            {
                Debug.Log("CreationChainStrategySystem: " + entity);
                
                if (!cooldownAspect.Targets.Read(entity).Value.TryGetID(out int creatableID)) 
                    continue;
                
                CreatableAspect creatableAspect = _world.GetAspect<CreatableAspect>();

                if (!creatableAspect.IsMatches(creatableID))
                    continue;

                ref GameObjectConnect goConnect = ref creatableAspect.GoConnects.Get(creatableID);
                goConnect.Connect.gameObject.SetActive(true);
            }
        }
    }
}