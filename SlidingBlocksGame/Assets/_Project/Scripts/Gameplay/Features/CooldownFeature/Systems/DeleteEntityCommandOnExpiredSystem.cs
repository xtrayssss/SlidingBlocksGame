using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Systems
{
    public class DeleteEntityCommandOnExpiredSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] private readonly EcsTagPool<CooldownExpiredMarker> _expired;
            [Inc] private readonly EcsTagPool<DeleteOnExpiredMarker> _deleteOnExpired;

            [Opt] public readonly EcsTagPool<DeleteEntityCommand> DeleteCommands;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect)) 
                aspect.DeleteCommands.Add(entity);
        }
    }
}