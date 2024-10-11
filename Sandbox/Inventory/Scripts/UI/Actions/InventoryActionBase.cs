using Godot;
using System;

namespace Template.Inventory;

public abstract class InventoryActionBase()
{
    private event Action<InventoryActionEventArgs> _onPreAction;
    protected event Action<InventoryActionEventArgs> _onPostAction;

    protected InventoryContext _context { get; private set; }
    protected MouseButton _mouseButton { get; private set; }
    protected int _index { get; private set; }

    public void Initialize(InventoryContext context, MouseButton mouseBtn, int index, Action<InventoryActionEventArgs> onPreAction, Action<InventoryActionEventArgs> onPostAction)
    {
        _context = context;
        _mouseButton = mouseBtn;
        _index = index;
        _onPreAction = onPreAction;
        _onPostAction = onPostAction;
    }

    protected void InvokeOnPreAction(InventoryActionEventArgs args)
    {
        _onPreAction?.Invoke(args);
    }

    protected void InvokeOnPostAction(InventoryActionEventArgs args)
    {
        _onPostAction?.Invoke(args);
    }

    public abstract void Execute();
}
