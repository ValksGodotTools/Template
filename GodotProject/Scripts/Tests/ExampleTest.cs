using GdUnit4;
using Godot;
using Template.TopDown2D;
using static GdUnit4.Assertions;

namespace Template.Tests;

[TestSuite]
public partial class ExampleTest
{
    private Frog _frog;

    [BeforeTest]
    public void Setup()
    {
        _frog = Game.LoadPrefab<Frog>(Prefab.Frog);
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
