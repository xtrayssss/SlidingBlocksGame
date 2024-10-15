using _Project.Scripts.Gameplay.Features.DestructionFeature.Components;
using _Project.Scripts.Gameplay.Features.GameProgressFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameProgressFeature.IntegrationFeatures.DestructionFeature.Systems
{
    public class CoinDestroySystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class DestroyedCoinAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CoinTag> CoinTag;
            [Inc] public readonly EcsTagPool<DiedEvent> DiedEvent;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out DestroyedCoinAspect _)) 
                _world.DelEntity(entity);
        }
    }
}