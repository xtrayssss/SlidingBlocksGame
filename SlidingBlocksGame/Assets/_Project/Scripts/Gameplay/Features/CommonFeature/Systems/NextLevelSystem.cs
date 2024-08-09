using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.DestroyFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class NextLevelRequestSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<LevelWinMarker> Obstacles;
        }

        private class GameAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(Levels))]
            [Opt] public readonly EcsTagPool<NextLeveRequest> NextLevelRequest;
        }

        public void Run()
        {
            foreach (int _ in _world.Where(out Aspect _))
            {
                foreach (int game in _world.Where(out GameAspect gameAspect))
                    gameAspect.NextLevelRequest.Add(game);
            }
        }
    }

    public class NextLevelSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(NextLeveRequest))]
            [Inc] public readonly EcsPool<Levels> Levels;

            [Inc] public readonly EcsPool<LevelIndex> LevelIndices;
        }
        
        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log("Next level system");

                ref readonly Levels levels = ref aspect.Levels.Read(entity);

                ref LevelIndex levelIndex = ref aspect.LevelIndices.Get(entity);

                levelIndex.Value++;

                ScriptableEntityTemplate[] levelsPack = GetLevelsPack(levels, levelIndex);

                if (aspect.LevelIndices.Get(entity).Value >= levelsPack.Length - 1)
                {
                    levelIndex.Value = 0;
                    levelIndex.Pack++;
                }

                ScriptableEntityTemplate nextLevelCfg = levelsPack[levelIndex.Value];

                entlong nextLevel = _world.NewEntityLong(nextLevelCfg);

                _world.GetTagPool<CreateLevelRequest>().Add(nextLevel.ID);
            }
        }

        private ScriptableEntityTemplate[] GetLevelsPack(Levels levels, LevelIndex levelIndex) =>
            levels.Value[levelIndex.Value].Levels;
    }
}