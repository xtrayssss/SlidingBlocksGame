using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class NextLevelSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [IncImplicit(typeof(NextLeveRequest))]
            [Inc] public readonly EcsPool<Levels> Levels;

            [Inc] public readonly EcsPool<GameScreen> GameScreens;
        }

        private Random random = new Random((uint)Environment.TickCount);

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log("Next level system");

                ref Levels levels = ref aspect.Levels.Get(entity);

                ScriptableEntityTemplate[] levelsPack = GetLevelsPack(in levels);

                if (levels.LevelIndex >= levelsPack.Length)
                {
                    levels.LevelIndex = 0;
                    levels.PackIndex++;
                }

                ScriptableEntityTemplate nextLevelCfg = levelsPack[levels.LevelIndex];

                entlong nextLevel = _world.NewEntityLong(nextLevelCfg);

                _world.GetTagPool<SpawnedEvent>().Add(nextLevel.ID);

                aspect.GameScreens.Add(nextLevel.ID).Value = aspect.GameScreens.Read(entity).Value;

                levels.LevelIndex = random.NextInt(0, levelsPack.Length - 1);

                levels.LevelIndex++;
            }
        }

        private ScriptableEntityTemplate[] GetLevelsPack(in Levels levels) =>
            levels.Value[levels.PackIndex].Levels;
    }
}