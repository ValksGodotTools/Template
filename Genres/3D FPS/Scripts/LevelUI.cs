namespace Template;

public partial class LevelUI : Node
{
    UIPopupMenu popupMenu;

    public override void _Ready()
    {
        popupMenu = GetNode<UIPopupMenu>("%PopupMenu");
        popupMenu.OnOpened += () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        };
        popupMenu.OnClosed += () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        };

        Input.MouseMode = Input.MouseModeEnum.Captured;

        UIConsole console = Global.Services.Get<UIConsole>();

        console.OnToggleVisibility += HandleConsoleToggled;

        popupMenu.OnMainMenuBtnPressed += () =>
        {
            // No longer need to listen for this
            console.OnToggleVisibility -= HandleConsoleToggled;
        };
    }

    void HandleConsoleToggled(bool visible)
    {
        SetPhysicsProcess(!visible);
        SetProcessInput(!visible);

        if (visible)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            if (!popupMenu.Visible)
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }
}
