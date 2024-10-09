using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameOverTimerFeature
{
    public class GameOverTimerFeature : IEcsModule
    {
        public void Import(EcsPipeline.Builder builder)
        {
           builder 
               .AddAudioSystem<CooldownTickEvent, TickAudioConfig>()
        }
    }
}