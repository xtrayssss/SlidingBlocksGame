using System;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class Extent : MonoBehaviour
    {
        private void Update()
        {
            UnityEngine.Debug.Log(GetComponent<MeshRenderer>().bounds.extents);
        }
    }
}