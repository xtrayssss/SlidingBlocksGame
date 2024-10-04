using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.PurchaseFeature.Systems;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.UIFeature.Systems
{
    public class Render3DToUISystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(Render3DToUIRequest))]
            [Inc] public readonly EcsPool<TargetEntity> Renderables;
            [Inc] public readonly EcsPool<RawImageRef> RawImages;
        }

        private class RenderableAspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<Renderable3DTexture> Renderable3DTextures;
            [Opt] public readonly EcsPool<RenderCamera> RenderCamera;
            [Opt] public readonly EcsPool<PhysicView> PhysicViews;
        }

        private class GameAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<Renderer3DCameraPrefab> Renderer3DCameraPrefabs;
        }

        private static int _counter;
        private const int STEP = 250;
        private const float FORWARD_OFFSET = 25;

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                if (!aspect.Renderables.Read(entity).Value.TryGetID(out int renderableID))
                    continue;

                RenderableAspect renderableAspect = _world.GetAspect<RenderableAspect>();

                ref PhysicView physicView = ref renderableAspect.PhysicViews.Get(renderableID);

                _counter++;

                physicView.Value.transform.position = new Vector3(_counter * STEP, 0, 0);

                ref Renderable3DTexture renderable3DTexture =
                    ref renderableAspect.Renderable3DTextures.Add(renderableID);

                renderable3DTexture.Value = new RenderTexture(
                    width: 512,
                    height: 512,
                    depth: 16);

                foreach (int game in _world.Where(out GameAspect gameAspect))
                {
                    ref readonly Renderer3DCameraPrefab
                        cameraPrefab = ref gameAspect.Renderer3DCameraPrefabs.Read(game);
                    
                    Camera camera = Object.Instantiate(cameraPrefab.Value);

                    renderableAspect.RenderCamera.Add(renderableID).Value = camera; 

                    camera.targetTexture = renderable3DTexture.Value;

                    ref RawImageRef rawImage = ref aspect.RawImages.Get(entity);

                    rawImage.Value.texture = renderable3DTexture.Value;

                    Bounds bounds = GetBounds(physicView.Value.gameObject);

                    float orthographicSize = CalculateOrthographicSize(bounds, camera);

                    camera.orthographicSize = orthographicSize;

                    float distance = bounds.center.z - bounds.extents.z + FORWARD_OFFSET;
                    camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, distance);

                    camera.transform.LookAt(bounds.center);

                    camera.Render();
                }
            }
        }

        private static Bounds GetBounds(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            
            Bounds bounds = new Bounds
            {
                center = renderers[0].bounds.center,
                size = Vector3.zero
            };

            foreach (Renderer renderer in renderers) 
                bounds.Encapsulate(renderer.bounds);

            return bounds;
        }

        private static float CalculateOrthographicSize(Bounds bounds, Camera camera)
        {
            float objectHeight = bounds.size.y;
            float objectWidth = bounds.size.x;

            float aspectRatio = camera.aspect;

            float orthographicSize = objectHeight / 2f;

            if (objectWidth / aspectRatio > objectHeight)
                orthographicSize = objectWidth / aspectRatio / 2f;

            return orthographicSize;
        }
    }
}