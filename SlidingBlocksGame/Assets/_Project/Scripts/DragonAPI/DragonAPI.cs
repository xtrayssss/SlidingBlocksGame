using System.Collections.Generic;
using UnityEditor;

namespace _Project.Scripts.DragonAPI
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class DragonAPI
    {
        private static readonly List<DragonCoroutineRunner> COROUTINE_RUNNERS = new List<DragonCoroutineRunner>();

        static DragonAPI() =>
            COROUTINE_RUNNERS.Clear();

        public static DragonCoroutineRunner CreateCoroutineRunner()
        {
            DragonCoroutineRunner dragonCoroutineRunner =
                new DragonCoroutineRunner(ownerIndex: COROUTINE_RUNNERS.Count);
            COROUTINE_RUNNERS.Add(dragonCoroutineRunner);
            return dragonCoroutineRunner;
        }

        public static void StopCoroutine(DragonCoroutine dragonCoroutine)
        {
            int index = dragonCoroutine.OwnerIndex;
            DragonCoroutineRunner dragonCoroutineRunner = COROUTINE_RUNNERS[index];
            dragonCoroutineRunner.StopCoroutine(dragonCoroutine);
        }
    }
}