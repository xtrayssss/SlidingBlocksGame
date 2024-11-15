using DCFApixels.DragonECS;

namespace _Project.Scripts.Infrastructure
{
    public interface IAlwaysRun : IEcsProcess
    {
        public void Run();
    }
}