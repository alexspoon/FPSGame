using Godot;
using System;

public partial class InventorySlot : TextureRect
{
    public int[,] InventoryPosition;
    private Label _debugLabel = new();
    private InventoryMenu _inventoryMenu;

    public override void _Ready()
    {
        _inventoryMenu = GetParent().GetParent().GetParent().GetParent<InventoryMenu>();
        _debugLabel.SetAnchorsPreset(LayoutPreset.Center);
        
        _debugLabel.Text = InventoryPosition.ToString();
        AddChild(_debugLabel);
    }
}
