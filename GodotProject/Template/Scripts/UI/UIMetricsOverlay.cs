namespace Template;

using Monitor = Performance.Monitor;

public partial class UIMetricsOverlay : Control
{
    Label labelFPS;
    Label labelMinRAM;
    Label labelMaxRAM;
    Label labelVidRAM;
    Label labelNodes;
    Label labelOrphanNodes;

    public override void _Ready()
    {
        labelFPS = GetNode<Label>("%FPS");
        labelMinRAM = GetNode<Label>("%RAM Min");
        labelMaxRAM = GetNode<Label>("%RAM Max");
        labelVidRAM = GetNode<Label>("%Vid RAM");
        labelNodes = GetNode<Label>("%Nodes");
        labelOrphanNodes = GetNode<Label>("%Orphan Nodes");

        if (GOS.IsExportedRelease())
        {
            labelMinRAM.Hide();
            labelMaxRAM.Hide();
        }

        Hide();
        SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        RenderPerformanceMetrics();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("debug_overlay"))
        {
            Visible = !Visible;
            SetPhysicsProcess(Visible);
        }
    }

    void RenderPerformanceMetrics()
    {
        const int BYTES_IN_MEGABYTE = 1048576;

        labelFPS.Text = Engine.GetFramesPerSecond().ToString();

        if (!GOS.IsExportedRelease())
        {
            double minRAM = Performance.GetMonitor(Monitor.MemoryStatic) / BYTES_IN_MEGABYTE;
            double maxRAM = Performance.GetMonitor(Monitor.MemoryStaticMax) / BYTES_IN_MEGABYTE;

            labelMinRAM.Text = $"{minRAM:0.0}";
            labelMaxRAM.Text = $"{maxRAM:0.0}";
        }

        double vidRAM = Performance.GetMonitor(Monitor.RenderVideoMemUsed) / BYTES_IN_MEGABYTE;

        labelVidRAM.Text = $"{vidRAM:0.0}";
        labelNodes.Text = Performance.GetMonitor(Monitor.ObjectNodeCount).ToString();
        labelOrphanNodes.Text = Performance.GetMonitor(Monitor.ObjectOrphanNodeCount).ToString();
    }
}
