using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using NotImplementedException = System.NotImplementedException;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class CellOccupancySystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredMarker))]
            [IncImplicit(typeof(MovementCommand))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<CellOccupancyMarker> CellOccupancyMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (aspect.Targets.Read(entity).Value.TryGetID(out int id))
                {
                    aspect.CellOccupancyMarker.Add(id);
                }
            }
        }
    }

    public class System : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredMarker))]
            [IncImplicit(typeof(MovementCommand))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<CellOccupancyMarker> CellOccupancyMarker;
        }

        private class UnavailabilityAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<CellDestination> CellDestinations;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                foreach (int unavailability in _world.Where(out UnavailabilityAspect unavailabilityAspect))
                {
                    ref readonly CellDestination cellDestination = ref unavailabilityAspect.CellDestinations.Read(unavailability);
                    
                    
                    
                }
            }
        }
    }
}