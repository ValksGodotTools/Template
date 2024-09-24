using Godot;
using System;
using System.Collections.Generic;

namespace Template.InventoryV2;

[SceneTree]
public partial class InventoryContainer : PanelContainer
{
    public void AddItemContainer()
    {
        ItemContainer itemContainer = ItemContainer.Instantiate();
        GridContainer.AddChild(itemContainer);
    }

    [OnInstantiate]
    private void Init()
    {
        
    }
}
