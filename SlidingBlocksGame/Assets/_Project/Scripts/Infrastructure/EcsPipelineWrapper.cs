using DCFApixels.DragonECS;

namespace _Project.Scripts.Infrastructure
{
    public class EcsPipelineWrapper
    {
        private readonly EcsPipeline _pipeline;
        private readonly EcsModule _rootModule;

        private EcsPipelineWrapper(EcsPipeline pipeline, EcsModule rootModule)
        {
            _pipeline = pipeline;
            _rootModule = rootModule;
        }

        public static Builder New(IConfigContainerWriter config = null) =>
            new Builder(new EcsPipeline.Builder(config));

        public void Destroy() => _pipeline.Destroy();

        public void UpdateRun(EcsWorld world) => _rootModule.Update(world);

        public class Builder
        {
            private readonly EcsPipeline.Builder _builder;
            private EcsModule _root;

            public Builder(EcsPipeline.Builder builder) =>
                _builder = builder;

            public Builder AddUnityDebug(params EcsWorld[] worlds)
            {
                _builder.AddUnityDebug(worlds);
                return this;
            }

            public Builder AutoInject(bool isAggressiveInjection = false)
            {
                _builder.AutoInject(isAggressiveInjection);
                return this;
            }

            public Builder Inject<T>(T data)
            {
                _builder.Inject(data);
                return this;
            }

            public Builder AddRoot(EcsModule rootModule)
            {
                _root = rootModule;
                _root.Initialize(_builder);
                return this;
            }

            public EcsPipelineWrapper Build() => 
                new EcsPipelineWrapper(_builder.BuildAndInit(), _root);
        }
    }
}