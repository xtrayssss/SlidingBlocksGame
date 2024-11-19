using System;
using UnityEngine;

namespace _Project.Scripts.DragonAPI.YieldInstructions
{
    public static partial class DragonAPI
    {
        public sealed class WaitUntil<TTarget> : CustomYieldInstruction
        {
            private readonly Func<TTarget, bool> _condition;
            private readonly TTarget _target;

            public WaitUntil(TTarget target, Func<TTarget, bool> condition)
            {
                _target = target;
                _condition = condition;
            }

            public override bool keepWaiting => !_condition(_target);
        }

        public sealed class WaitUntil : CustomYieldInstruction
        {
            private readonly Func<bool> _condition;

            public WaitUntil(Func<bool> condition) =>
                _condition = condition;

            public override bool keepWaiting => !_condition();
        }
    }
}