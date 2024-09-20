using Godot;

namespace Template;

public partial class CommandLineArgs : Node
{
    public override void _Ready()
    {
        // Get the command-line arguments
        string[] args = OS.GetCmdlineArgs();

        // Initialize the window position and size
        Vector2I windowPosition = Vector2I.Zero;
        Vector2I windowSize = Vector2I.Zero;

        // Get the screen size
        Vector2 screenSize = DisplayServer.ScreenGetSize();

        // Define the vertical space for the window bar
        int windowBarHeight = 30; // Adjust this value based on your actual window bar height

        // Loop through the arguments to find the position argument
        foreach (string arg in args)
        {
            if (arg == "top_left")
            {
                windowPosition = new Vector2I(0, windowBarHeight);
                windowSize = new Vector2I((int)(screenSize.X / 2), (int)((screenSize.Y - windowBarHeight) / 2));
                break;
            }
            else if (arg == "top_right")
            {
                windowPosition = new Vector2I((int)(screenSize.X / 2), windowBarHeight);
                windowSize = new Vector2I((int)(screenSize.X / 2), (int)((screenSize.Y - windowBarHeight) / 2));
                break;
            }
            else if (arg == "bottom_left")
            {
                windowPosition = new Vector2I(0, (int)(screenSize.Y / 2) + windowBarHeight);
                windowSize = new Vector2I((int)(screenSize.X / 2), (int)((screenSize.Y - windowBarHeight) / 2));
                break;
            }
            else if (arg == "bottom_right")
            {
                windowPosition = new Vector2I((int)(screenSize.X / 2), (int)(screenSize.Y / 2) + windowBarHeight);
                windowSize = new Vector2I((int)(screenSize.X / 2), (int)((screenSize.Y - windowBarHeight) / 2));
                break;
            }
            else if (arg == "middle_left")
            {
                windowPosition = new Vector2I(0, windowBarHeight);
                windowSize = new Vector2I((int)(screenSize.X / 2), (int)(screenSize.Y - windowBarHeight));
                break;
            }
            else if (arg == "middle_right")
            {
                windowPosition = new Vector2I((int)(screenSize.X / 2), windowBarHeight);
                windowSize = new Vector2I((int)(screenSize.X / 2), (int)(screenSize.Y - windowBarHeight));
                break;
            }
            else if (arg == "middle_top")
            {
                windowPosition = new Vector2I(0, windowBarHeight);
                windowSize = new Vector2I((int)screenSize.X, (int)((screenSize.Y - windowBarHeight) / 2));
                break;
            }
            else if (arg == "middle_bottom")
            {
                windowPosition = new Vector2I(0, (int)(screenSize.Y / 2) + windowBarHeight);
                windowSize = new Vector2I((int)screenSize.X, (int)((screenSize.Y - windowBarHeight) / 2));
                break;
            }
        }

        // Set the window size
        DisplayServer.WindowSetSize(windowSize);

        // Set the window position
        DisplayServer.WindowSetPosition(windowPosition);
    }
}
