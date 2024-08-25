using System;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldAlgorithmsFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Systems
{
    public class CalculateGameFieldSystem : IEcsRun
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class LevelAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(GameFieldGenerateRequest))]
            [Inc] public readonly EcsPool<GameField> GameFields;
        }

        private class PlayerAspect : EcsAspectAuto
        {
            [IncImplicit(typeof(PlayerTag))]
            [Inc] public readonly EcsPool<SelectionAnimalID> SelectionAnimalIndicies;
        }

        private class GameAspect : EcsAspectAuto
        {
            [Inc] public readonly EcsPool<AnimalPrefabs> AnimalPrefabs;
        }

        public void Run()
        {
            foreach (int level in _world.Where(out LevelAspect levelAspect))
            {
                ref GameField field = ref levelAspect.GameFields.Get(level);

                field.EdgeSize = field.Size / 3;
                field.CenterSize = field.Size - 2 * field.EdgeSize;

                // TODO: move to another system
                foreach (int player in _world.Where(out PlayerAspect playerAspect))
                {
                    foreach (int game in _world.Where(out GameAspect gameAspect))
                    {
                        AnimalEntityConnect animalPrefab = gameAspect.AnimalPrefabs.Read(game).Animals[playerAspect.SelectionAnimalIndicies.Read(player).Value];

                        foreach (ref GameField.Unit unit in levelAspect.GameFields.Get(level).Units.AsSpan())
                            unit.Prefab = animalPrefab;
                    }
                }
            }
        }
    }
}