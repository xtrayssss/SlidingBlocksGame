using _Project.Scripts.Gameplay.Features.GameFlowFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.GameFlowFeature.Systems
{
    public class NextLevelSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(NextLeveRequest))]
            [Inc] public readonly EcsPool<Levels> Levels;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                ref Levels levels = ref aspect.Levels.Get(entity);

                ScriptableEntityTemplate[] levelsPack = GetLevelsInPack(ref levels);

                int random = levels.Randoms[levels.PackIndex][levels.LevelsCount];

                ScriptableEntityTemplate randomLevelCfg = levelsPack[random];

                entlong nextLevel = _world.NewEntityLong(randomLevelCfg);

                _world.GetTagPool<LevelChangedEvent>().Add(nextLevel.ID);

                levels.LevelsCount++;
            }
        }
        
        private ScriptableEntityTemplate[] GetLevelsInPack(ref Levels levels)
        {
            ScriptableEntityTemplate[] levelsPack = levels.Pack[levels.PackIndex].Levels;

            if (levels.LevelsCount >= levelsPack.Length)
            {
                levels.PackIndex++;
                levels.LevelsCount = 0;

                if (levels.PackIndex >= levels.Pack.Length)
                {
                    levels.PackIndex = 0;

                    return levels.Pack[levels.PackIndex].Levels;
                }

                levelsPack = levels.Pack[levels.PackIndex].Levels;
            }

            return levelsPack;
        }
    }
}