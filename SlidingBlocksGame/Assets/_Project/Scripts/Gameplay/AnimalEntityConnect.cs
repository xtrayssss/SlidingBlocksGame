using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class AnimalEntityConnect : EcsEntityConnect
    {
        [SerializeField] private GameObject _renderer;

        public GameObject Renderer => _renderer;
    }
}