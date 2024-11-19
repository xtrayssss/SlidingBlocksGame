using System.Linq;
using UnityEngine;

namespace _Project.Scripts.DragonAPI.YieldInstructions
{
    public static partial class DragonAPI
    {
        public sealed class WhenAll : CustomYieldInstruction
        {
            private readonly DragonCoroutine[] _coroutines;

            public WhenAll(DragonCoroutine[] coroutines) =>
                _coroutines = coroutines;

            public override bool keepWaiting => _coroutines.Any(coroutine => coroutine.IsAlive);
        }
    }
}