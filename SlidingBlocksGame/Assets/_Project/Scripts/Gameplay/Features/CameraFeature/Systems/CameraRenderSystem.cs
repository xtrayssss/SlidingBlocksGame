using _Project.Scripts.Gameplay.Features.CameraFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.CameraFeature.Systems
{
    public class CameraRenderSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(ViewUpdatedMarker))]
            [Inc] public readonly EcsPool<RenderCamera> RenderCameras;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref readonly RenderCamera renderCamera = ref aspect.RenderCameras.Read(entity);

                renderCamera.Value.Render();
            }
        }
    }
}