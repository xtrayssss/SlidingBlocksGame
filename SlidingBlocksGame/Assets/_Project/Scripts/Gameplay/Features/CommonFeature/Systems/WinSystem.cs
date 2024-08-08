using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.CommonFeature.Systems
{
    public class WinSystem : IEcsRun
    {
        [EcsInject] private readonly EcsDefaultWorld _world;

        private class Aspect : EcsAspectAuto
        {
            [Inc] public readonly EcsTagPool<CellOccupancyMarker> Obstacles1;
            [Inc] public readonly EcsTagPool<WithinCenterMarker> Obstacles2;
        }

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(LevelTag))]
            [Exc] public readonly EcsTagPool<LevelWinMarker> LevelWin;
        }

        public void Run()
        {
            foreach (int entity in _world.Where(out LevelAspect levelAspect))
            {
                if (_world.Where(out Aspect _).Count == 4)
                {
                    Debug.Log("WINNER");
                    levelAspect.LevelWin.Add(entity);
                }
            }
        }
    }
}