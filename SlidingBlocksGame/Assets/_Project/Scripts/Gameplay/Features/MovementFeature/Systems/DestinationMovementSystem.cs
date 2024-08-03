using System.Collections.Generic;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.EasingFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class DestinationMovementSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class EasingAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(MovementCommand))]
            [Inc] public readonly EcsPool<EasingDestination> Destinations;

            [Inc] public readonly EcsPool<TargetEntity> Targets;
        }

        private class MovementAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<WorldPosition> WorldPositions;
            [Exc] public readonly EcsTagPool<UpdateViewRequest> UpdateView;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out EasingAspect easingAspect))
            {
                if (easingAspect.Targets.Read(entity).Value.TryGetID(out int id))
                {
                    MovementAspect movementAspect = _world.GetAspect<MovementAspect>();

                    if(_world.GetPool<UpdateViewRequest>().Has(id) == false && movementAspect.IsMatches(id))
                    {
                        EcsDebug.Print("====================================");
                        EcsDebug.Print(_world.GetEntityLong(id));

                        EcsAspect aspect = _world.GetAspect<MovementAspect>();
                        EcsDebug.Print(aspect.Mask.ToString());
                        EcsDebug.Print("Inc " + aspect.Mask.Inc.ToArray());
                        EcsDebug.Print("Exc " + aspect.Mask.Exc.ToArray());

                        List<object> list = new List<object>();
                        _world.GetComponentsFor(id, list);
                        EcsDebug.Print(string.Join(',', list));

                        var ids = _world.GetComponentTypeIDsFor(id);
                        EcsDebug.Print(string.Join(',', ids.ToArray()));

                        EcsDebug.Print("====================================");
                    }
                    
                    if (movementAspect.IsMatches(id))
                    {   
                        movementAspect.WorldPositions.Get(id).Value = easingAspect.Destinations.Read(entity).Interpolation;
                        movementAspect.UpdateView.Add(id);
                    }
                }
            }
        }
    }
}