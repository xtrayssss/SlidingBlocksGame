using DCFApixels.DragonECS;
using UnityEngine;

<<<<<<<< HEAD:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/AnimalEntityConnect.cs
namespace _Project.Scripts.Gameplay
========
namespace _Project.Scripts.Gameplay.Features.AnimalFeature.Components
>>>>>>>> recovery-branch:SlidingBlocksGame/Assets/_Project/Scripts/Gameplay/Features/AnimalFeature/Components/AnimalEntityConnect.cs
{
    public class AnimalEntityConnect : EcsEntityConnect
    {
        [SerializeField] private GameObject _renderer;

        public GameObject Renderer => _renderer;
    }
}