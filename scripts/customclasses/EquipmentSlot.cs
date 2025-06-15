using Godot;

[GlobalClass]
public partial class EquipmentSlot : Panel
{
    public bool Hovered;
	[Export] public InventoryItem.EquipmentTypes SlotType;
	[Export] public InventoryItem EquippedItem = null;
    private Label _slotLabel;

    public override void _Ready()
    {
        _slotLabel = GetNode<Label>("EquipmentLabel");
        _slotLabel.Owner = this;
        OnSlotChange();
        MouseEntered += MouseEnter;
        MouseExited += MouseExit;
    }

    public void OnSlotChange()
    {
        if (EquippedItem == null) _slotLabel.Text = "Nothing";
        else _slotLabel.Text = EquippedItem.Name;
    }

    private void MouseEnter()
    {
        Hovered = true;
    }

    private void MouseExit()
    {
        Hovered = false;
    }
}
