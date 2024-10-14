using System.Collections;
using System.Collections.Generic;

namespace _Project.Scripts.DragonAPI
{
    public class DragonCoroutineRunner
    {
        private readonly int _ownerIndex;
        private readonly List<DragonCoroutine> _coroutines = new List<DragonCoroutine>();

        public DragonCoroutineRunner(int ownerIndex) =>
            _ownerIndex = ownerIndex;

        public void Tick()
        {
            for (int index = 0; index < _coroutines.Count; index++)
            {
                DragonCoroutine coroutine = _coroutines[index];

                if (!coroutine.MoveNext()) 
                    _coroutines.RemoveAt(index);
            }
        }

        public DragonCoroutine StartCoroutine(IEnumerator coroutine)
        {
            DragonCoroutine dragonCoroutine = new DragonCoroutine(coroutine, _ownerIndex);

            _coroutines.Add(dragonCoroutine);

            coroutine.MoveNext();

            return dragonCoroutine;
        }

        public void StopCoroutine(DragonCoroutine dragonCoroutine) =>
            _coroutines.Remove(dragonCoroutine);
    }
}