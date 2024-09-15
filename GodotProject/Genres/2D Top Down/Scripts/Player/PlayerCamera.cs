using Godot;

using Template.TopDown2D;

namespace Template;

public partial class PlayerCamera : Camera2D
{
    Player player;

	public override void _Ready()
	{
        PositionSmoothingEnabled = false;
        SetPhysicsProcess(false);
    }

	public override void _PhysicsProcess(double delta)
	{
        Position = player.Position;
    }

    public void StartFollowingPlayer(Player player)
    {
        this.player = player;
        Position = player.Position;
        GTween.Delay(this, 0.01, () => PositionSmoothingEnabled = true);
        SetPhysicsProcess(true);
    }

    public void StopFollowingPlayer()
    {
        SetPhysicsProcess(false);
    }
}

