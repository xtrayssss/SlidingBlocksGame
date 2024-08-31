using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public interface ICoroutineRunner
    {
        public Coroutine StartCoroutine(IEnumerator coroutine);
    }
}