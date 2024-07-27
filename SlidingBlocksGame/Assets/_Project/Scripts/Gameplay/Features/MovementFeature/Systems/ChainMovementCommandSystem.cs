using System.Runtime.InteropServices;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.MovementFeature.Systems
{
    public class ChainMovementCommandSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(CooldownExpiredMarker))]
            [IncImplicit(typeof(ChainMovementMarker))]
            [Inc] public readonly EcsPool<TargetEntity> Targets;

            [Opt] public readonly EcsTagPool<MovementCommand> Commands;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log("Block movement command");

                if (aspect.Targets.Read(entity).Value.TryGetID(out int id))
                    aspect.Commands.Add(id);
            }
        }
    }
}