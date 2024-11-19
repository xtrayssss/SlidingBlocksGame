using UnityEngine;

namespace _Project.Scripts.DragonAPI.YieldInstructions
{
    public static partial class DragonAPI
    {
        public sealed class WaitForSeconds : CustomYieldInstruction
        {
            private float _delay;

            public WaitForSeconds(float delay) =>
                _delay = delay;

            public override bool keepWaiting => (_delay -= Time.deltaTime) > 0f;
        }
    }
}