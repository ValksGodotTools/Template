using Godot;

namespace Template.InventoryV2;

[SceneTree]
public partial class InventoryContainer : PanelContainer
{
    public ItemContainer AddItemContainer()
    {
        ItemContainer itemContainer = ItemContainer.Instantiate();
        GridContainer.AddChild(itemContainer);
        return itemContainer;
    }

    [OnInstantiate]
    private void Init()
    {
        
    }
}
