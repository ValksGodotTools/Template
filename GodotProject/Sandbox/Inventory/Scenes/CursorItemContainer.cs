using CSharpUtils;
using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class CursorItemContainer : Node2D
{
    public Inventory Inventory { get; private set; }

    private const float InitialSmoothFactor = 0.05f;
    private const float LerpBackToOneFactor = 0.01f;

    private ItemContainer _itemContainer;
    private Vector2 _offset;
    private float _currentSmoothFactor;
    private State _currentState;

    public override void _Ready()
    {
        Inventory = new(1);
        Inventory.OnItemChanged += HandleItemChanged;

        _itemContainer = _.ItemContainer;
        IgnoreInputEvents(_itemContainer);

        _offset = _itemContainer.CustomMinimumSize * 0.5f;
        _currentSmoothFactor = InitialSmoothFactor;

        _currentState = Idle();

        Show();
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState.Update((float)delta);
    }

    public void SetItemAndFrame(Item item, int frame)
    {
        Inventory.SetItem(0, item);
        _itemContainer.SetCurrentSpriteFrame(frame);
        _currentState = MoveTowardsCursor();
    }

    public void GetItemAndFrame(out Item item, out int frame)
    {
        item = Inventory.GetItem(0);
        frame = _itemContainer.GetCurrentSpriteFrame();
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
        _currentState = Idle();
    }

    public new void SetPosition(Vector2 position)
    {
        _itemContainer.Position = position;
    }

    public void ResetSmoothFactor()
    {
        _currentSmoothFactor = InitialSmoothFactor;
    }

    private void HandleItemChanged(int index, Item item)
    {
        _itemContainer.SetItem(item);
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

    private State MoveTowardsCursor()
    {
        State state = new(nameof(MoveTowardsCursor));

        state.Update = delta =>
        {
            Vector2 target = GetGlobalMousePosition() - _offset;
            float distance = _itemContainer.Position.DistanceTo(target);

            _itemContainer.Position = _itemContainer.Position.MoveToward(target, distance * _currentSmoothFactor);
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
