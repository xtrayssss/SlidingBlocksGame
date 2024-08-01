using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class CalculateDestinationCellIntersectionSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            //[IncImplicit(typeof(CalculateDestinationCellRequest))]
            [IncImplicit(typeof(DestinationUnavailableMarker))]
            [Inc] public readonly EcsPool<CellDestination> CellDestinations;

            [Inc] public readonly EcsPool<MovementDirection> Directions;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref CellDestination cellDestination = ref aspect.CellDestinations.Get(entity);

                cellDestination.Value -= aspect.Directions.Read(entity).Value;
            }
        }
    }
}