using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class LevelGeneratedMarkerSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameField))]
            [IncImplicit(typeof(CreateGameRequest))]
            [Opt] public readonly EcsTagPool<LevelGenerateMarker> Marker;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
                aspect.Marker.Add(entity);
        }
    }
}