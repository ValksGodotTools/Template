global using Godot;
global using GodotUtils;
global using System;
global using System.Collections.Generic;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Threading;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;

namespace Template;

public partial class Main : Node
{
	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Logger.Update();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			GetTree().AutoAcceptQuit = false;
			HandleCleanup();
		}
	}

	private void HandleCleanup()
	{
		// Handle cleanup here

		GetTree().Quit();
	}
}
