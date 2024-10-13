using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CooldownFeature.Systems
{
    public class DeleteEntityOnExpiredSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CooldownExpiredMarker> CooldownExpiredMarkerMarker;
            [Inc] public readonly EcsTagPool<DeleteOnExpiredMarker> DeleteOnExpiredMarker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect _)) 
                _world.DelEntity(entity);
        }
    }
}