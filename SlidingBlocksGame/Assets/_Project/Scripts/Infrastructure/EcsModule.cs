using System.Collections.Generic;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Infrastructure
{
    public abstract class EcsModule
    {
        private List<IEcsRun> _runners;
        private List<EcsModule> _submodules;

        protected abstract void Import(Builder builder);

        public void Initialize(EcsPipeline.Builder pipelineBuilder)
        {
            var builder = new Builder();
            Import(builder);

            _runners = builder.Runners;
            _submodules = builder.Submodules;

            foreach (var system in builder.Systems)
            {
                pipelineBuilder.Add(system);
            }

            foreach (var submodule in _submodules)
            {
                submodule.Initialize(pipelineBuilder);
            }
        }

        public virtual void Update(EcsWorld world)
        {
            foreach (var runner in _runners)
            {
                runner.Run();
            }

            foreach (var submodule in _submodules)
            {
                submodule.Update(world);
            }
        }

        public class Builder
        {
            public List<IEcsProcess> Systems { get; } = new List<IEcsProcess>();
            public List<IEcsRun> Runners { get; } = new List<IEcsRun>();
            public List<EcsModule> Submodules { get; } = new List<EcsModule>();

            public Builder AddSystem<T>() where T : IEcsProcess, new()
            {
                var system = new T();
                Systems.Add(system);

                if (system is IEcsRun runner)
                {
                    Runners.Add(runner);
                }

                return this;
            }

            public Builder AutoDelTag<T>() where T : struct, IEcsTagComponent
            {
                DeleteOneFrameTagComponentSystem<T> system = new DeleteOneFrameTagComponentSystem<T>();

                Systems.Add(system);

                if (system is IEcsRun runner)
                {
                    Runners.Add(runner);
                }

                return this;
            }

            public Builder AddUnique(IEcsProcess system)
            {
                Systems.Add(system);

                if (system is IEcsRun runner)
                {
                    Runners.Add(runner);
                }

                return this;
            }

            public Builder AutoDelEntityTag<TComponent>() where TComponent : struct, IEcsTagComponent
            {
                DeleteOneFrameEntityTagSystem<TComponent> system = new DeleteOneFrameEntityTagSystem<TComponent>();

                Systems.Add(system);

                if (system is IEcsRun runner)
                {
                    Runners.Add(runner);
                }

                return this;
            }

            public Builder AutoDelEntityComponent<TComponent>() where TComponent : struct, IEcsComponent
            {
                DeleteOneFrameEntityComponentSystem<TComponent> system = new DeleteOneFrameEntityComponentSystem<TComponent>();

                Systems.Add(system);

                if (system is IEcsRun runner)
                {
                    Runners.Add(runner);
                }

                return this;
            }
            
            public Builder AutoDel<TComponent>() where TComponent : struct, IEcsComponent
            {
                DeleteOneFrameComponentSystem<TComponent> system = new DeleteOneFrameComponentSystem<TComponent>();

                Systems.Add(system);

                if (system is IEcsRun runner)
                {
                    Runners.Add(runner);
                }

                return this;
            }

            public Builder AddSubmodule<T>() where T : EcsModule, new()
            {
                Submodules.Add(new T());
                return this;
            }

            public Builder AddSubmodule(EcsModule module)
            {
                Submodules.Add(module);
                return this;
            }
        }
    }

    public abstract class EcsModule<TMask> : EcsModule
        where TMask : EcsAspect, new()
    {
        public override void Update(EcsWorld world)
        {
            if (world.Where(out TMask _).Count > 0)
            {
                base.Update(world);
            }
        }
    }
}