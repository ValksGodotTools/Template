namespace GdUnit4.TestAdapter;

public partial class TestAdapterRunner : Api.TestRunner
{
    public override void _Ready()
        => _ = RunTests();
}
