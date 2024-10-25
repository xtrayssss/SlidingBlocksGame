using _Project.Scripts.Gameplay.Features.CameraFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CameraFeature.Systems
{
    public class Object3DPreviewSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class RenderRequestAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<TargetEntity> Renderables;
            [Inc] public readonly EcsPool<ObjectPreviewRequest> Render3DToUIRequest;
        }

        private class RenderableAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<RenderCamera> RenderCamera;
            [Opt] public readonly EcsPool<PhysicView> PhysicViews;
        }

        private class HolderCameraAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<PreviewCameraPrefab> CameraPrefabs;
        }

        private static int RENDER_POSITION_COUNTER;
        private const int POSITION_STEP = 250;
        private const float CAMERA_OFFSET = 25f;
        private const int TEXTURE_SIZE = 512;
        private const int TEXTURE_DEPTH = 16;

        public void Run()
        {
            foreach (int request in _world.Where(out RenderRequestAspect renderRequest))
            {
                if (!renderRequest.Renderables.Read(request).Value.TryGetID(out int renderableID))
                    return;

                RenderableAspect renderableAspect = _world.GetAspect<RenderableAspect>();

                PositionRenderable(renderableID, renderableAspect);

                RenderTexture renderTexture = CreateRenderTexture();

                SetupRenderCamera(renderableID, renderableAspect, renderTexture, renderRequest, request);
            }
        }

        private static void PositionRenderable(int renderableID, RenderableAspect renderableAspect)
        {
            ref PhysicView physicView = ref renderableAspect.PhysicViews.Get(renderableID);
            RENDER_POSITION_COUNTER++;
            physicView.Value.transform.position = new Vector3(RENDER_POSITION_COUNTER * POSITION_STEP, 0, 0);
        }

        private static RenderTexture CreateRenderTexture()
        {
            RenderTexture renderTexture = new RenderTexture(
                width: TEXTURE_SIZE,
                height: TEXTURE_SIZE, 
                TEXTURE_DEPTH);
            
            return renderTexture;
        }

        private void SetupRenderCamera(
            int renderTargetID,
            RenderableAspect renderable,
            RenderTexture renderTexture,
            RenderRequestAspect renderRequest,
            int requestEntity)
        {
            foreach (int holder in _world.Where(out HolderCameraAspect holderCameraAspect))
            {
                Camera camera = CreateCamera(holder, holderCameraAspect, renderTargetID, renderable);
                ConfigureCamera(camera, renderTexture, renderRequest, requestEntity, renderable, renderTargetID);
                RenderScene(camera);
            }
        }

        private static Camera CreateCamera(
            int holder,
            HolderCameraAspect holderCameraAspect,
            int renderableID,
            RenderableAspect renderableAspect)
        {
            ref readonly PreviewCameraPrefab prefab = ref holderCameraAspect.CameraPrefabs.Read(holder);
            Camera camera = Object.Instantiate(prefab.Value);
            renderableAspect.RenderCamera.Add(renderableID).Value = camera;
            return camera;
        }

        private static void ConfigureCamera(
            Camera camera,
            RenderTexture texture,
            RenderRequestAspect requestAspect,
            int request,
            RenderableAspect renderableAspect,
            int renderableID)
        {
            camera.targetTexture = texture;
            requestAspect.Render3DToUIRequest.Get(request).RawImage.texture = texture;

            ref PhysicView physicView = ref renderableAspect.PhysicViews.Get(renderableID);
            Bounds objectBounds = CalculateObjectBounds(physicView.Value.gameObject);
            PositionCameraForTarget(camera, in objectBounds);
        }

        private static void PositionCameraForTarget(Camera camera, in Bounds bounds)
        {
            float cameraDistance = bounds.center.z - bounds.extents.z + CAMERA_OFFSET;
            camera.transform.position = new Vector3(
                bounds.center.x,
                bounds.center.y,
                cameraDistance);

            camera.transform.LookAt(bounds.center);
        }

        private static void RenderScene(Camera camera) => 
            camera.Render();

        private static Bounds CalculateObjectBounds(GameObject target)
        {
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

            Bounds bounds = new Bounds
            {
                center = renderers[0].bounds.center,
                size = Vector3.zero
            };

            foreach (Renderer renderer in renderers)
                bounds.Encapsulate(renderer.bounds);

            return bounds;
        }
    }
}