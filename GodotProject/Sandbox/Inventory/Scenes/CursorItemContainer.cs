using CSharpUtils;
using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class CursorItemContainer : Node2D
{
    public Inventory Inventory { get; private set; }
    public ItemContainer ItemContainer { get; private set; }
    public State CurrentState { get; set; }

    private const float InitialSmoothFactor = 0.05f;
    private const float LerpBackToOneFactor = 0.01f;

    private Vector2 _offset;
    private float _currentSmoothFactor;

    public override void _Ready()
    {
        Inventory = new(1);
        Inventory.OnItemChanged += HandleItemChanged;

        ItemContainer = _.ItemContainer;
        IgnoreInputEvents(ItemContainer);

        _offset = ItemContainer.CustomMinimumSize * 0.5f;
        _currentSmoothFactor = InitialSmoothFactor;

        CurrentState = Idle();

        Show();
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentState.Update((float)delta);
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
        CurrentState = Idle();
    }

    public new void SetPosition(Vector2 position)
    {
        ItemContainer.Position = position;
    }

    public void ResetSmoothFactor()
    {
        _currentSmoothFactor = InitialSmoothFactor;
    }

    private void HandleItemChanged(int index, Item item)
    {
        ItemContainer.SetItem(item);
    }

    private static void IgnoreInputEvents(Control control)
    {
        control.MouseFilter = Control.MouseFilterEnum.Ignore;
        control.SetProcessInput(false);
    }

    private State Idle()
    {
        State state = new(nameof(Idle));
        return state;
    }

    public State MoveTowardsCursor()
    {
        State state = new(nameof(MoveTowardsCursor));

        state.Update = delta =>
        {
            Vector2 target = GetGlobalMousePosition() - _offset;
            float distance = ItemContainer.Position.DistanceTo(target);

            ItemContainer.Position = ItemContainer.Position.MoveToward(target, distance * _currentSmoothFactor);
            _currentSmoothFactor = Mathf.Lerp(_currentSmoothFactor, 1, LerpBackToOneFactor);
        };

        return state;
    }

    private State MoveTowardsContainer()
    {
        State state = new(nameof(MoveTowardsContainer));

        state.Update = delta =>
        {
            // TODO: Implement move towards container
        };

        return state;
    }
}
