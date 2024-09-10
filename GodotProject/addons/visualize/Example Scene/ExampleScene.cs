using Godot;
using System;
using Visualize.Utils;

namespace Visualize.Example;

public partial class ExampleScene : Node
{
	public override void _Ready()
	{
		PackedScene packedScene = GD.Load<PackedScene>("res://addons/visualize/Example Scene/sprite_2d.tscn");
		Sprite2D sprite = packedScene.Instantiate<Sprite2D>();

		// As you can see the visualize info is created at the moment of node creation
		_ = new GTween(this)
			.Delay(1)
			.Callback(() => AddChild(sprite));
	}
}
