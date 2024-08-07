using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.DestroyFeature.Systems
{
    public class AnimalDestructionChainStrategySystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class AnimalAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(AnimalTag))]
            [ExcImplicit(typeof(CellOccupancyMarker))]
            [ExcImplicit(typeof(Obstacle))]
            private int _;
        }

        private class ChainAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestructionChainTag))]
            [ExcImplicit(typeof(TargetEntity))]
            [Opt] public readonly EcsPool<Chain> Chains;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out ChainAspect aspect))
            {
                ref Chain chain = ref aspect.Chains.Add(entity);

                chain.Value = EcsGroup.New(_world);

                foreach (int animal in _world.Where(out AnimalAspect _))
                    chain.Value.Add(animal);
            }
        }
    }
}