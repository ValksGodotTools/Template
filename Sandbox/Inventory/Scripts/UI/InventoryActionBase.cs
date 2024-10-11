using Godot;
using System;

namespace Template.Inventory;

public abstract class InventoryActionBase()
{
    protected event Action<InventoryActionEventArgs> OnPreAction;
    protected event Action<InventoryActionEventArgs> OnPostAction;

    protected InventoryContext Context { get; private set; }
    protected MouseButton MouseButton { get; private set; }
    protected int Index { get; private set; }

    public void Initialize(InventoryContext context, MouseButton mouseBtn, int index, Action<InventoryActionEventArgs> onPreAction, Action<InventoryActionEventArgs> onPostAction)
    {
        Context = context;
        MouseButton = mouseBtn;
        Index = index;
        OnPreAction = onPreAction;
        OnPostAction = onPostAction;
    }

    protected void InvokeOnPreAction(InventoryActionEventArgs args)
    {
        OnPreAction?.Invoke(args);
    }

    protected void InvokeOnPostAction(InventoryActionEventArgs args)
    {
        OnPostAction?.Invoke(args);
    }

    public abstract void Execute();
}
