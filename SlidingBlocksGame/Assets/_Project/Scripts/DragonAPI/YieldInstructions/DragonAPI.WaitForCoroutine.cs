using UnityEngine;

namespace _Project.Scripts.DragonAPI.YieldInstructions
{
    public static partial class DragonAPI
    {
        public sealed class WaitForCoroutine : CustomYieldInstruction
        {
            private readonly DragonCoroutine _coroutine;

            public WaitForCoroutine(DragonCoroutine coroutine) =>
                _coroutine = coroutine;

            public override bool keepWaiting => _coroutine.IsAlive;
        }
    }
}