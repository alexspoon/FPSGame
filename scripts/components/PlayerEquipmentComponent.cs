using Godot;

public partial class PlayerEquipmentComponent : Node
{
    private Node _componentContainer;
    private CharacterBody3D _player;
    private Level _currentLevel;
    private Node _levelObjectPool;
    private Node3D _head;
    private RayCast3D _headRay;
    private Control _hud;
    private Panel _equipmentMenu;
    private InventoryMenu _inventoryMenu;
    private ContextMenu _contextMenu;
    private Marker3D _interactPoint;
    public bool MenuVisible;

    private EquipmentSlot _equipmentSlotPrimary;
    private EquipmentSlot _equipmentSlotSecondary;
    private EquipmentSlot _equipmentSlotMelee;
    private EquipmentSlot _equipmentSlotBack;
    private EquipmentSlot _equipmentSlotHead;
    private EquipmentSlot _equipmentSlotChest;
    private EquipmentSlot _equipmentSlotLegs;
    private EquipmentSlot[] _allSlots;

    public override void _Ready()
    {
        GetReferences();
        _allSlots = new[]
        {
            _equipmentSlotPrimary, 
            _equipmentSlotSecondary, 
            _equipmentSlotMelee, 
            _equipmentSlotBack, 
            _equipmentSlotHead, 
            _equipmentSlotChest, 
            _equipmentSlotLegs
        };
    }

    private void GetReferences()
    {
        _componentContainer = GetParent<Node>();
        _player = _componentContainer.GetParent<CharacterBody3D>();
        _currentLevel = _player.GetParent<Level>();
        _levelObjectPool = _currentLevel.GetNode<Node>("ObjectPool");
        _head = _player.GetNode<Node3D>("Head");
        _headRay = _head.GetNode<RayCast3D>("HeadRay");
        _interactPoint = _head.GetNode<Marker3D>("InteractCollider/InteractPoint");
        _hud = _player.GetNode<Control>("HUD");
        _inventoryMenu = _hud.GetNode<InventoryMenu>("InventoryMenu");
        _contextMenu = _hud.GetNode<ContextMenu>("ContextMenu");
        _equipmentMenu = _hud.GetNode<Panel>("EquipmentMenu");
        _equipmentSlotPrimary = _equipmentMenu.GetNode<EquipmentSlot>("EquipmentSlotPrimary");
        _equipmentSlotSecondary = _equipmentMenu.GetNode<EquipmentSlot>("EquipmentSlotSecondary");
        _equipmentSlotMelee = _equipmentMenu.GetNode<EquipmentSlot>("EquipmentSlotMelee");
        _equipmentSlotBack = _equipmentMenu.GetNode<EquipmentSlot>("EquipmentSlotBack");
        _equipmentSlotHead = _equipmentMenu.GetNode<EquipmentSlot>("EquipmentSlotHead");
        _equipmentSlotChest = _equipmentMenu.GetNode<EquipmentSlot>("EquipmentSlotChest");
        _equipmentSlotLegs = _equipmentMenu.GetNode<EquipmentSlot>("EquipmentSlotLegs");
    }

    public override void _PhysicsProcess(double delta)
    {
        TryEquip();
        ContextMenuHandler();
        MenuVisibilityHandler();
    }

    private void MenuVisibilityHandler()
    {
        if (!Input.IsActionJustPressed("inputTab")) return;
        switch (_equipmentMenu.Visible)
        {
            case true: HideMenu(); break;
            case false: ShowMenu(); break;
        }
    }

    private void ShowMenu()
    {
        _equipmentMenu.Show();
        MenuVisible = true;
        Input.MouseMode = Input.MouseModeEnum.Confined;
    }

    private void HideMenu()
    {
        _equipmentMenu.Hide();
        MenuVisible = false;
        _contextMenu.HideMenu();
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private EquipmentSlot GetSlot(string equipmentType)
    {
        switch (equipmentType)
        {
            case "EquipmentPrimary" :  return _equipmentSlotPrimary;
            case "EquipmentSecondary" : return _equipmentSlotSecondary;
            case "EquipmentMelee" : return _equipmentSlotMelee;
            case "EquipmentBack" : return _equipmentSlotBack;
            case "EquipmentHead" : return _equipmentSlotHead;
            case "EquipmentChest" : return _equipmentSlotChest;
            case "EquipmentLegs" : return _equipmentSlotLegs; 
        }
        return null;
    }

    public void DropItem(InventoryItem item)
    {
        var slot = GetSlot(item.EquipmentType.ToString());
        var prefab = item.Prefab;
        var rigid = prefab.Instantiate<RigidBody3D>();
        rigid.LinearVelocity = -_head.GlobalTransform.Basis.Z * 5;
        _levelObjectPool.AddChild(rigid);
        rigid.GlobalRotation = -_head.GlobalRotation;
        rigid.GlobalPosition = _interactPoint.GlobalPosition;
        if (item.HasInventory) _inventoryMenu.CloseInventory();
        item.Equipped = false;
        slot.EquippedItem = null;
        slot.OnSlotChange();
    }

    public bool TryEquipItem(InventoryItem item)
    {
        if (item.Equippable == false) return false;
        GD.Print("Item is equippable");
        var slot = GetSlot(item.EquipmentType.ToString());
        slot.EquippedItem = item;
        item.Equipped = true;
        slot.OnSlotChange();
        GD.Print("Equipped " + item.Name);
        return true;
    }

    public bool TryEquipFromGround(RigidBody3D targetObject)
    {
        var itemComponent = targetObject.GetNodeOrNull<ItemComponent>("ItemComponent");
        if (itemComponent == null) return false;
        // GD.Print("Object has ItemComponent");
        var item = itemComponent.Item;
        if (item == null) return false;
        if (!item.Equippable) return false;
        // GD.Print("Item is equippable");
        var slot = GetSlot(item.EquipmentType.ToString());
        if (slot.EquippedItem != null) DropItem(slot.EquippedItem);
        slot.EquippedItem = item;
        item.Equipped = true;
        slot.OnSlotChange();
        // GD.Print("Equipped " + item.Name);
        return true;
    }

    private void TryEquip()
    {
        if (!Input.IsActionJustPressed("inputF")) return;
        if (!_headRay.IsColliding()) return;
        var collider = _headRay.GetCollider() as RigidBody3D;
        if (collider == null) return;
        if (TryEquipFromGround(collider)) collider.Free();
    }

    private void ContextMenuHandler()
    {
        if (_equipmentMenu.Visible == false) return;
        if (!Input.IsActionJustPressed("inputRightMouse")) return;
        for (int i = 0; i < _allSlots.Length; i++)
        {
            var slot = _allSlots[i];
            if (!slot.Hovered) continue;
            if (slot.EquippedItem == null)
            {
                // GD.Print("slot " + slot.SlotType.ToString() + " empty");
                continue;
            }
            _contextMenu.SelectItem(slot.EquippedItem);
            // GD.Print("item " + slot.EquippedItem.Name + " selected");
            break;
        }
    }
}
