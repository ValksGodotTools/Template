using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class CursorItemContainer : ItemContainer
{
    public CursorInventory Inventory { get; private set; }

    private const float InitialSmoothFactor = 0.05f;
    private const float LerpBackToOneFactor = 0.01f;

    private Vector2 _offset;
    private float _currentSmoothFactor;

    public override void _Ready()
    {
        Inventory = new();
        Inventory.OnItemChanged += HandleItemChanged;

        IgnoreInputEvents(this);

        _offset = CustomMinimumSize * 0.5f;
        _currentSmoothFactor = InitialSmoothFactor;

        SetPhysicsProcess(false);

        Show();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 target = GetGlobalMousePosition() - _offset;
        float distance = Position.DistanceTo(target);

        Position = Position.MoveToward(target, distance * _currentSmoothFactor);
        _currentSmoothFactor = Mathf.Lerp(_currentSmoothFactor, 1, LerpBackToOneFactor);
    }

    public Item GetItem()
    {
        return Inventory.GetItem();
    }

    public bool HasItem()
    {
        return Inventory.HasItem();
    }

    public void ClearItem()
    {
        Inventory.ClearItem();
        SetPhysicsProcess(false);
    }

    public void ResetSmoothFactor()
    {
        _currentSmoothFactor = InitialSmoothFactor;
    }

    private void HandleItemChanged(Item item)
    {
        SetItem(item);
    }

    private static void IgnoreInputEvents(Control control)
    {
        control.MouseFilter = MouseFilterEnum.Ignore;
        control.SetProcessInput(false);
    }
}
