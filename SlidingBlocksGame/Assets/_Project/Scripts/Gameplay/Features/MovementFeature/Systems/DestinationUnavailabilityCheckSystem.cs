using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class DestinationUnavailabilityCheckSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class SectionAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(DestinationUnavailabilityCheckRequest))]
            [Inc] public readonly EcsPool<CellDestination> Destinations;

            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<DestinationUnavailableMarker> DestinationUnavailableMarker;
        }

        private class BlockAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(BlockTag))]
            [ExcImplicit(typeof(DestinationUnavailableMarker))]
            [Inc] public readonly EcsPool<CellDestination> CellDestinations;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out SectionAspect aspect))
            {
                ref readonly CellDestination destination = ref aspect.Destinations.Read(entity);

                if (aspect.Targets.Read(entity).Value.TryGetID(out int id))
                {
                    foreach (int block in _world.Where(out BlockAspect blockAspect))
                    {
                        ref readonly CellDestination blockDestination = ref blockAspect.CellDestinations.Read(block);

                        if (block != id && math.all(blockDestination.Value == destination.Value))
                        {
                            Debug.Log("intersected!" + destination.Value + id + " " + blockDestination.Value + block);

                            aspect.DestinationUnavailableMarker.Add(id);
                            
                            break;
                        }
                    }
                }
            }
        }
    }
}