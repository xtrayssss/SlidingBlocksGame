using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class WinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> Obstacles1;
            [Inc] public readonly EcsTagPool<WithinCenterMarker> Obstacles2;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Exc] public readonly EcsTagPool<LevelWinMarker> LevelWin;
        }

        private class AnimalAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AnimalTag> Levels;
        }

        private class ChainAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestructionChainTag))]
            [ExcImplicit(typeof(TargetEntity))]
            [Opt] public readonly EcsPool<Chain> Chains;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out LevelAspect levelAspect))
            {
                if (_world.Where(out Aspect _).Count == 4)
                {
                    Debug.Log("WINNER");
                    levelAspect.LevelWin.Add(entity);
                    //_world.GetPool<NextLeveRequest>().Add(entity);

                    // TODO: rework
                    
                    ref readonly DestructionAnimalStrategyCfg destructionAnimalStrategyCfg =
                        ref _world.GetPool<DestructionAnimalStrategyCfg>().Read(entity);

                    int chainID = _world.NewEntity(destructionAnimalStrategyCfg.Value);

                    ChainAspect chainAspect = _world.GetAspect<ChainAspect>();

                    ref Chain chain = ref chainAspect.Chains.Add(chainID);

                    chain.Value = EcsGroup.New(_world);

                    foreach (int animal in _world.Where(out AnimalAspect _))
                        chain.Value.Add(animal);
                }
            }
        }
    }
}