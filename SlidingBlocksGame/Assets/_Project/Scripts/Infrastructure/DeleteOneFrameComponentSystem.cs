using System.Collections.Generic;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Infrastructure
{
    [MetaTags(MetaTags.HIDDEN)]
    [MetaColor(MetaColor.Grey)]
    public class DeleteOneFrameComponentSystem<TComponent> : IEcsRun, IEcsInject<EcsWorld>
        where TComponent : struct, IEcsComponent
    {
        private sealed class Aspect : EcsAspect
        {
            public EcsPool<TComponent> pool = Inc;
        }

        private readonly List<EcsWorld> _worlds = new List<EcsWorld>();
        public void Inject(EcsWorld obj) => _worlds.Add(obj);

        public void Run()
        {
            for (int i = 0, iMax = _worlds.Count; i < iMax; i++)
            {
                EcsWorld world = _worlds[i];
                if (world.IsComponentTypeDeclared<TComponent>())
                {
                    foreach (var e in world.Where(out Aspect a))
                    {
                        a.pool.Del(e);
                    }
                }
            }
        }
    }

    [MetaTags(MetaTags.HIDDEN)]
    [MetaColor(MetaColor.Grey)]
    public class DeleteOneFrameTagComponentSystem<TComponent> : IEcsRun, IEcsInject<EcsWorld>
        where TComponent : struct, IEcsTagComponent
    {
        private sealed class Aspect : EcsAspect
        {
            public EcsTagPool<TComponent> pool = Inc;
        }

        private readonly List<EcsWorld> _worlds = new List<EcsWorld>();
        public void Inject(EcsWorld obj) => _worlds.Add(obj);

        public void Run()
        {
            for (int i = 0, iMax = _worlds.Count; i < iMax; i++)
            {
                EcsWorld world = _worlds[i];
                if (world.IsComponentTypeDeclared<TComponent>())
                {
                    foreach (var e in world.Where(out Aspect a))
                    {
                        a.pool.Del(e);
                    }
                }
            }
        }
    }

    [MetaTags(MetaTags.HIDDEN)]
    [MetaColor(MetaColor.Grey)]
    public class DeleteOneFrameEntityTagSystem<TComponent> : IEcsRun, IEcsInject<EcsWorld>
        where TComponent : struct, IEcsTagComponent
    {
        private sealed class Aspect : EcsAspect
        {
            public EcsTagPool<TComponent> pool = Inc;
        }

        private readonly List<EcsWorld> _worlds = new List<EcsWorld>();
        public void Inject(EcsWorld obj) => _worlds.Add(obj);

        public void Run()
        {
            for (int i = 0, iMax = _worlds.Count; i < iMax; i++)
            {
                EcsWorld world = _worlds[i];
                if (world.IsComponentTypeDeclared<TComponent>())
                {
                    foreach (var e in world.Where(out Aspect a))
                    {
                        world.DelEntity(e);
                    }
                }
            }
        }
    }
    [MetaTags(MetaTags.HIDDEN)]
    [MetaColor(MetaColor.Grey)]
    public class DeleteOneFrameEntityComponentSystem<TComponent> : IEcsRun, IEcsInject<EcsWorld>
        where TComponent : struct, IEcsComponent
    {
        private sealed class Aspect : EcsAspect
        {
            public EcsPool<TComponent> pool = Inc;
        }

        private readonly List<EcsWorld> _worlds = new List<EcsWorld>();
        public void Inject(EcsWorld obj) => _worlds.Add(obj);

        public void Run()
        {
            for (int i = 0, iMax = _worlds.Count; i < iMax; i++)
            {
                EcsWorld world = _worlds[i];
                if (world.IsComponentTypeDeclared<TComponent>())
                {
                    foreach (var e in world.Where(out Aspect a))
                    {
                        world.DelEntity(e);
                    }
                }
            }
        }
    }
}