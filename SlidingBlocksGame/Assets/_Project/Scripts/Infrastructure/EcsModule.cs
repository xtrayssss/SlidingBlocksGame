using System.Collections.Generic;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Infrastructure
{
    public abstract class EcsModule : IEcsModule
    {
        // private readonly EcsWorld _world;
        // public readonly EcsMask.Builder Builder;
        // public readonly EcsPipeline.Builder PipelineBuilder;
        // public List<IEcsRun> Runs = new List<IEcsRun>();
        // public List<IEcsInit> Inits = new List<IEcsInit>();
        // public EcsMask Mask;
        //
        // public EcsModule(EcsWorld world, EcsPipeline.Builder pipelineBuilder)
        // {
        //     _world = world;
        //     PipelineBuilder = pipelineBuilder;
        //     Builder = EcsMask.New(world);
        // }
        //
        // public EcsModule Build()
        // {
        //     Mask = Builder.Build();
        //
        //     return this;
        // }
        //
        // public void Run()
        // {
        //     if (_world.Where(Mask).Count <= 0)
        //         return;
        //
        //     foreach (var ecsRun in Runs)
        //         ecsRun.Run();
        // }
        //
        // public EcsModule AddUnique<TProcess>(TProcess process) where TProcess : IEcsProcess
        // {
        //     if (process is IEcsRun run) 
        //         Runs.Add(run);
        //     else if (process is IEcsInit init) 
        //         Inits.Add(init);
        //     
        //     PipelineBuilder.AddUnique(process);
        //     
        //     return this;
        // }
        //
        // public EcsModule AutoDelTag<TTag>() where TTag : struct, IEcsTagComponent
        // {
        //     PipelineBuilder.AutoDelTag<TTag>();
        // }
        //
        // public abstract void Import(EcsPipeline.Builder b);
        public void Import(EcsPipeline.Builder b)
        {
            
        }
    }
}