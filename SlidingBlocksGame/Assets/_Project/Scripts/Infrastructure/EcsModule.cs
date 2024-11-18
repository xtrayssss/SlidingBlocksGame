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
            Builder moduleBuilder = new Builder();

            Import(moduleBuilder);

            _runners = moduleBuilder.Runners;
            _submodules = moduleBuilder.Submodules;

            foreach (IEcsProcess system in moduleBuilder.Systems)
                pipelineBuilder.Add(system);

            foreach (EcsModule submodule in _submodules)
                submodule.Initialize(pipelineBuilder);
        }

        public virtual void Update(EcsWorld world)
        {
            foreach (IEcsRun runner in _runners)
                runner.Run();

            foreach (EcsModule submodule in _submodules)
                submodule.Update(world);
        }

        public class Builder
        {
            public List<IEcsProcess> Systems { get; } = new List<IEcsProcess>();
            public List<IEcsRun> Runners { get; } = new List<IEcsRun>();
            public List<EcsModule> Submodules { get; } = new List<EcsModule>();

            public Builder AutoDelTag<T>() where T : struct, IEcsTagComponent
            {
                DeleteOneFrameTagComponentSystem<T> system = new DeleteOneFrameTagComponentSystem<T>();

                Systems.Add(system);

                if (system is IEcsRun runner) 
                    Runners.Add(runner);

                return this;
            }

            public Builder AddSystem(IEcsProcess system)
            {
                Systems.Add(system);

                if (system is IEcsRun runner) 
                    Runners.Add(runner);

                return this;
            }

            public Builder AutoDelEntityTag<TComponent>() where TComponent : struct, IEcsTagComponent
            {
                DeleteOneFrameEntityTagSystem<TComponent> system = new DeleteOneFrameEntityTagSystem<TComponent>();

                Systems.Add(system);

                if (system is IEcsRun runner) 
                    Runners.Add(runner);

                return this;
            }

            public Builder AutoDelEntityComponent<TComponent>() where TComponent : struct, IEcsComponent
            {
                DeleteOneFrameEntityComponentSystem<TComponent> system =
                    new DeleteOneFrameEntityComponentSystem<TComponent>();

                Systems.Add(system);

                if (system is IEcsRun runner) 
                    Runners.Add(runner);

                return this;
            }

            public Builder AutoDel<TComponent>() where TComponent : struct, IEcsComponent
            {
                DeleteOneFrameComponentSystem<TComponent> system = new DeleteOneFrameComponentSystem<TComponent>();

                Systems.Add(system);

                if (system is IEcsRun runner) 
                    Runners.Add(runner);

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
                base.Update(world);
        }
    }
}