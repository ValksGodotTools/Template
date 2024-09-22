using Godot;
using GodotUtils;
using Monitor = Godot.Performance.Monitor;

namespace Template;

public partial class UIMetricsOverlay : Control
{
    private Label _labelFPS;
    private Label _labelMinRAM;
    private Label _labelMaxRAM;
    private Label _labelVidRAM;
    private Label _labelNodes;
    private Label _labelOrphanNodes;

    public override void _Ready()
    {
        _labelFPS = GetNode<Label>("%FPS");
        _labelMinRAM = GetNode<Label>("%RAM Min");
        _labelMaxRAM = GetNode<Label>("%RAM Max");
        _labelVidRAM = GetNode<Label>("%Vid RAM");
        _labelNodes = GetNode<Label>("%Nodes");
        _labelOrphanNodes = GetNode<Label>("%Orphan Nodes");

        if (GOS.IsExportedRelease())
        {
            _labelMinRAM.Hide();
            _labelMaxRAM.Hide();
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

    private void RenderPerformanceMetrics()
    {
        const int BYTES_IN_MEGABYTE = 1048576;

        _labelFPS.Text = Engine.GetFramesPerSecond().ToString();

        if (!GOS.IsExportedRelease())
        {
            double minRAM = Performance.GetMonitor(Monitor.MemoryStatic) / BYTES_IN_MEGABYTE;
            double maxRAM = Performance.GetMonitor(Monitor.MemoryStaticMax) / BYTES_IN_MEGABYTE;

            _labelMinRAM.Text = $"{minRAM:0.0}";
            _labelMaxRAM.Text = $"{maxRAM:0.0}";
        }

        double vidRAM = Performance.GetMonitor(Monitor.RenderVideoMemUsed) / BYTES_IN_MEGABYTE;

        _labelVidRAM.Text = $"{vidRAM:0.0}";
        _labelNodes.Text = Performance.GetMonitor(Monitor.ObjectNodeCount).ToString();
        _labelOrphanNodes.Text = Performance.GetMonitor(Monitor.ObjectOrphanNodeCount).ToString();
    }
}

