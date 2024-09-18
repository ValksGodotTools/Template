using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

namespace Template.Tests;

[TestSuite]
public partial class ExampleTest
{
    private RigidBody2D _frog;

    [BeforeTest]
    public void Setup()
    {
        _frog = Game.LoadPrefab<RigidBody2D>(Prefab.Frog);
    }

    [TestCase]
    public void Test()
    {
        AssertThat(_frog.Modulate).IsEqual(Colors.White);
    }

    [AfterTest]
    public void Finished()
    {
        _frog.QueueFree();
    }
}
