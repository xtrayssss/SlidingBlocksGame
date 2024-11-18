using System;
using System.Collections;
using System.Collections.Generic;

namespace _Project.Scripts.DragonAPI
{
    public class DragonCoroutineRunner
    {
        private readonly int _ownerIndex;
        private readonly HashSet<DragonCoroutine> _activeCoroutines;
        private readonly Predicate<DragonCoroutine> _removeWhere;

        public DragonCoroutineRunner(int ownerIndex, int initialCapacity = 16)
        {
            _ownerIndex = ownerIndex;
            _activeCoroutines = new HashSet<DragonCoroutine>(initialCapacity);
            _removeWhere = static coroutine => !coroutine.MoveNext();
        }

        public void Tick() => 
            _activeCoroutines.RemoveWhere(_removeWhere);

        public DragonCoroutine StartCoroutine(IEnumerator coroutine)
        {
            DragonCoroutine dragonCoroutine = new DragonCoroutine(coroutine, _ownerIndex);
            _activeCoroutines.Add(dragonCoroutine);
            coroutine.MoveNext();
            return dragonCoroutine;
        }

        public void StopCoroutine(DragonCoroutine dragonCoroutine) =>
            _activeCoroutines.Remove(dragonCoroutine);
    }
}