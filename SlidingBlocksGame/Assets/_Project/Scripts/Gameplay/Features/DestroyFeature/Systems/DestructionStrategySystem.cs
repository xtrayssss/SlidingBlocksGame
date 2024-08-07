using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class DestructionStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestructionStrategyRequest))]
            [Inc] public readonly EcsPool<DestructionAnimalStrategyCfg> DestructionAnimalStrategyConfigs;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log("123");
                
                ref readonly DestructionAnimalStrategyCfg strategyCfg =
                    ref aspect.DestructionAnimalStrategyConfigs.Read(entity);

                _world.NewEntity(strategyCfg.Value);
            }
        }
    }
}