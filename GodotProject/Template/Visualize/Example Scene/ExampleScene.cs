using Godot;

namespace Template.Example;

public partial class ExampleScene : Node
{
	Camera2D camera;

	private const int CAMERA_SPEED = 5;

	public override void _Ready()
	{
		camera = GetNode<Camera2D>("Camera2D");

		PackedScene packedScene = GD.Load<PackedScene>("res://addons/visualize/Example Scene/sprite_2d.tscn");
		Sprite2D sprite = packedScene.Instantiate<Sprite2D>();

        // As you can see the visualize info is created at the moment of node creation
        _ = new GTween(this)
			.Delay(0.1)
			.Callback(() =>
			{
                AddChild(sprite);
            });
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key)
		{
			if (key.Keycode == Key.A)
			{
				camera.Position -= new Vector2(CAMERA_SPEED, 0);
			}

			if (key.Keycode == Key.D)
			{
				camera.Position += new Vector2(CAMERA_SPEED, 0);
			}

			if (key.Keycode == Key.W)
			{
				camera.Position -= new Vector2(0, CAMERA_SPEED);
			}

			if (key.Keycode == Key.S)
			{
				camera.Position += new Vector2(0, CAMERA_SPEED);
			}
		}
    }
}
