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

            [Inc] public readonly EcsPool<LevelCounter> LevelCounter;
            [Inc] public readonly EcsPool<GameScreen> GameScreens;
        }
        
        private Random random = new Random((uint)Environment.TickCount);

        public void Run()
        {
            foreach (int entity in _world.Where(out Aspect aspect))
            {
                Debug.Log("Next level system");

                ref readonly Levels levels = ref aspect.Levels.Read(entity);

                ref LevelCounter levelCounter = ref aspect.LevelCounter.Get(entity);

                ScriptableEntityTemplate[] levelsPack = GetLevelsPack(levels, levelCounter);

                if (aspect.LevelCounter.Get(entity).Value >= levelsPack.Length)
                {
                    levelCounter.Value = 0;
                    levelCounter.Pack++;
                }
                
                ScriptableEntityTemplate nextLevelCfg = levelsPack[levelCounter.Value];

                entlong nextLevel = _world.NewEntityLong(nextLevelCfg);

                _world.GetTagPool<CreateLevelRequest>().Add(nextLevel.ID);

                aspect.GameScreens.Add(nextLevel.ID).Value = aspect.GameScreens.Read(entity).Value;
                
                aspect.LevelCounter.Get(entity).Value = random.NextInt(0, levelsPack.Length - 1);

                levelCounter.Value++;
            }
        }

        private ScriptableEntityTemplate[] GetLevelsPack(Levels levels, LevelCounter levelCounter) =>
            levels.Value[levelCounter.Pack].Levels;
    }
}