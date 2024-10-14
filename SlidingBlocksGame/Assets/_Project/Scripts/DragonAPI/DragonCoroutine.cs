using System.Collections;
using UnityEngine;

namespace _Project.Scripts.DragonAPI
{
    public class DragonCoroutine
    {
        private IEnumerator _coroutine;
        public readonly int OwnerIndex;

        public DragonCoroutine(IEnumerator coroutine, int ownerIndex)
        {
            _coroutine = coroutine;
            OwnerIndex = ownerIndex;
        }

        public bool IsAlive => _coroutine != null;

        public bool MoveNext()
        {
            if (_coroutine.Current is null or CustomYieldInstruction { keepWaiting: false })
            {
                if (!_coroutine.MoveNext())
                    _coroutine = null;
            }

            return IsAlive;
        }
    }
}