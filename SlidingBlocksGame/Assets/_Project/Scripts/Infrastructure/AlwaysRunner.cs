using DCFApixels.DragonECS.RunnersCore;

namespace _Project.Scripts.Infrastructure
{
    public class AlwaysRunner : EcsRunner<IAlwaysRun>, IAlwaysRun
    {
        public void Run()
        {
            foreach (IAlwaysRun process in Process) 
                process.Run();
        }
    }
}