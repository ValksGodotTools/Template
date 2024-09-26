using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class CursorItemContainer : ItemContainer
{
    public Inventory Inventory { get; private set; }

    private const float InitialSmoothFactor = 0.05f;
    private const float LerpBackToOneFactor = 0.01f;

    private Vector2 _offset;
    private float _currentSmoothFactor;

    public override void _Ready()
    {
        Inventory = new(1);
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
        return Inventory.GetItem(0);
    }

    public bool HasItem()
    {
        return Inventory.HasItem(0);
    }

    public void ClearItem()
    {
        Inventory.ClearItem(0);
        SetPhysicsProcess(false);
    }

    public void ResetSmoothFactor()
    {
        _currentSmoothFactor = InitialSmoothFactor;
    }

    private void HandleItemChanged(int index, Item item)
    {
        SetItem(item);
    }

    private static void IgnoreInputEvents(Control control)
    {
        control.MouseFilter = Control.MouseFilterEnum.Ignore;
        control.SetProcessInput(false);
    }
}
