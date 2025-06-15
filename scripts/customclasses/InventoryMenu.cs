using Godot;

public partial class InventoryMenu : Panel
{
    private InventoryComponent _currentInventory;
    private VBoxContainer _inventoryContainer;
    private HBoxContainer _header;
    private Button _sortButton;
    private Button _spawnButton;
    private Button _closeButton;
    private ScrollContainer _scrollContainer;
    private GridContainer _inventoryGrid;
    [Export] private PackedScene _slot;

    public override void _Ready()
    {
        GetReferences();
        Hide();

        _closeButton.Pressed += CloseInventory;
    }

    private void GetReferences()
    {
        _inventoryContainer = GetNode<VBoxContainer>("InventoryContainer");
        _header = _inventoryContainer.GetNode<HBoxContainer>("Header");
        _sortButton = _header.GetNode<Button>("SortButton");
        _spawnButton = _header.GetNode<Button>("SpawnButton");
        _closeButton = _header.GetNode<Button>("CloseButton");
        _scrollContainer = _inventoryContainer.GetNode<ScrollContainer>("ScrollContainer");
        _inventoryGrid = _scrollContainer.GetNode<GridContainer>("InventoryGrid");
    }

    public void OpenInventory(InventoryItem item)
    {
        var inventory = item.Inventory;
        if (inventory.IsOpen) return;
        SpawnSlots(inventory);
        // var slotCount = inventory.Grid.LongLength;
        // SpawnSlots(slotCount);
        Show();
        inventory.IsOpen = true;
        _currentInventory = inventory;
    }

    public void CloseInventory()
    {
        CullSlots();
        _currentInventory.IsOpen = false;
        Hide();
    }

    private void CullSlots()
    {
        var slots = _inventoryGrid.GetChildren();
        foreach (TextureRect slot in slots)
        {
            slot.Free();
        }
    }

    private void SpawnSlots(InventoryComponent inventory)
    {
        for (int x = 0; x < inventory.Width; x++)
        {
            for (int y = 0; y < inventory.Height; y++)
            {
                var newSlot = _slot.Instantiate<InventorySlot>();
                var index = new int[x, y];
                newSlot.InventoryPosition = index;
                newSlot.Name = "slot" + index;
                _inventoryGrid.AddChild(newSlot);
                continue;
            }
        }
    }
}
