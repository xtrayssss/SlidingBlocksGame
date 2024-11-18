using System;
using System.Collections;
using System.Linq;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameFieldFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Components;
using _Project.Scripts.Gameplay.Features.MovementFeature.Systems;
using _Project.Scripts.Infrastructure;
using DCFApixels.DragonECS;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

[TestFixture]
public class CalculateDestinationSystemTests
{
    private EcsDefaultWorld _world;
    private EcsPipelineWrapper _pipeline;

    class Feature : EcsModule
    {
        protected override void Import(Builder builder)
        {
            builder.AddSystem(new CalculateDestinationSystem());
        }
    }

    [SetUp]
    public void SetUp()
    {
        _world = new EcsDefaultWorld();
        _pipeline = EcsPipelineWrapper.New()
            .AddRoot(new Feature())
            .Inject(_world)
            .AutoInject()
            .Build();
    }

    [TearDown]
    public void TearDown()
    {
        _pipeline.Destroy();
        _world.Destroy();

        Debug.Log("tear");
    }

    private entlong SetupGameField()
    {
        var level = _world.NewEntity();
        _world.GetPool<GameField>().Add(level) = new GameField
        {
            OriginPosition = new float3(-2.5f, 0, -1.8f),
            CellSize = 0.4666667f,
            Offset = 0.1f,
            Size = 9,
            EdgeSize = 3,
            CenterSize = 3,
            BaseSize = 6,
            BaseCellSize = 0.7f,
            CellsCount = 45,
            UnitCellTopOffset = 0.02f,
            Center = 0,
            BaseCellScaleY = 0.25f,
        };
        return level.ToEntityLong(_world);
    }

    private void CreateClickEvent(entlong gameField, float3 position, int2 expectedDirection)
    {
        var clickEntity = _world.NewEntity();
        _world.GetPool<SideClickedEvent>().Add(clickEntity);
        _world.GetPool<WorldPosition>().Add(clickEntity).Value = position;
        _world.GetPool<ActiveGameField>().Add(clickEntity).Value = gameField;
    }

    private entlong CreateAnimal(entlong gameField, int2 startPosition, int2 direction)
    {
        var animalEntity = _world.NewEntity();

        EcsEntityConnect view = Object.Instantiate(
            original: Resources.Load<EcsEntityConnect>("Bull"),
            position: new float3(-1.9333334f, 0.186666667f, 0.466666609f),
            rotation: quaternion.identity);

        view.ConnectWith(animalEntity.ToEntityLong(_world), applyTemplates: true);

        _world.GetPool<ActiveGameField>().Add(animalEntity).Value = gameField;
        _world.GetPool<CellPosition>().Add(animalEntity).Value = startPosition;
        _world.GetPool<MovementDirection>().Add(animalEntity).Value = direction;
        return animalEntity.ToEntityLong(_world);
    }

    [Test]
    public void CalculateDestination_NoValidMove_StaysInPlace()
    {
        // Arrange
        var gameField = SetupGameField();
        ref var field = ref _world.GetPool<GameField>().Get(gameField.ID);

        // Block all cells in movement direction
        for (int i = 0; i < field.CenterSize * field.CenterSize; i++)
        {
            field.Center |= (short)(1 << i);
        }

        CreateClickEvent(gameField, new float3(-1.9333334f, 0.186666667f, 0.466666609f), new int2(1, 0));
        var startPos = new int2(0, 3);
        var animal = CreateAnimal(gameField, startPos, new int2(1, 0));

        // Act
        _pipeline.UpdateRun(_world);

        // Assert
        var cellDest = _world.GetPool<CellDestination>().Read(animal.ID).Value;
        Debug.Log(cellDest);
        Assert.That(cellDest, Is.EqualTo(new int2(2, 3)), "Animal should not move when all cells are blocked");
    }

    [Test]
    public void CalculateDestination_MoveTowardsCenterFromEdge_SetsCorrectDestination()
    {
        // Arrange
        var gameField = SetupGameField();
        CreateClickEvent(gameField, new float3(-1.9333334f, 0.186666667f, 0.466666609f), new int2(1, 0));
        var animal = CreateAnimal(gameField, new int2(0, 3), new int2(1, 0));

        // Act
        _pipeline.UpdateRun(_world);

        // Assert
        Assert.IsTrue(_world.GetPool<WorldDestination>().Has(animal.ID));
        Assert.IsTrue(_world.GetPool<CellDestination>().Has(animal.ID));

        var cellDest = _world.GetPool<CellDestination>().Read(animal.ID).Value;
        Assert.That(cellDest.x, Is.GreaterThan(0), "Animal should move right towards center");
    }

    [Test]
    public void CalculateDestination_MultipleAnimalsMoving_HandlesGroupMovement()
    {
        // Arrange
        var gameField = SetupGameField();
        CreateClickEvent(gameField, new float3(-1.9333334f, 0.186666667f, 0.466666609f), new int2(1, 0));

        var animal1 = CreateAnimal(gameField, new int2(0, 3), new int2(1, 0));
        var animal2 = CreateAnimal(gameField, new int2(0, 4), new int2(1, 0));

        // Act
        _pipeline.UpdateRun(_world);

        // Assert
        Assert.IsTrue(_world.GetPool<WorldDestination>().Has(animal1.ID));
        Assert.IsTrue(_world.GetPool<WorldDestination>().Has(animal2.ID));

        var cellDest1 = _world.GetPool<CellDestination>().Read(animal1.ID).Value;
        var cellDest2 = _world.GetPool<CellDestination>().Read(animal2.ID).Value;

        Assert.That(cellDest1.x, Is.EqualTo(cellDest2.x), "Animals should move the same distance");
    }

    [TestCase(0, 3, Description = "Bottom edge position")]
    [TestCase(0, 4, Description = "Middle edge position")]
    [TestCase(0, 5, Description = "Top edge position")]
    [TestCase(1, 3, Description = "One step from edge bottom")]
    [TestCase(1, 4, Description = "One step from edge middle")]
    [TestCase(1, 5, Description = "One step from edge top")]
    public void CalculateDestination_BlockedByOccupiedCell_StopsBeforeCollision(int startX, int startY)
    {
        // Arrange
        var gameField = SetupGameField();
        ref var field = ref _world.GetPool<GameField>().Get(gameField.ID);

        // Set an occupied cell in the center area
        field.Center |= 1 << 4; // Set bit for occupied cell

        CreateClickEvent(gameField, new float3(-1.9333334f, 0.186666667f, 0.466666609f), new int2(1, 0));
        var animal = CreateAnimal(gameField, new int2(startX, startY), new int2(1, 0));

        // Act
        _pipeline.UpdateRun(_world);

        // Assert
        var cellDest = _world.GetPool<CellDestination>().Read(animal.ID).Value;
        Assert.That(cellDest.x, Is.LessThan(4), 
            $"Animal from position ({startX}, {startY}) should stop before occupied cell");
        Assert.That(cellDest.y, Is.EqualTo(startY), 
            $"Animal Y position should remain unchanged");
    }    [Test]
    public void CalculateDestination_DifferentDirections_OnlyMovesMatchingDirection()
    {
        // Arrange
        var gameField = SetupGameField();
        CreateClickEvent(gameField, new float3(-1.9333334f, 0.186666667f, 0.466666609f), new int2(1, 0));

        var matchingAnimal = CreateAnimal(gameField, new int2(0, 3), new int2(1, 0));
        var differentAnimal = CreateAnimal(gameField, new int2(0, 4), new int2(0, 1));

        // Act
        _pipeline.UpdateRun(_world);

        // Assert
        Assert.IsTrue(_world.GetPool<WorldDestination>().Has(matchingAnimal.ID));
        Assert.IsFalse(_world.GetPool<WorldDestination>().Has(differentAnimal.ID));
    }
}