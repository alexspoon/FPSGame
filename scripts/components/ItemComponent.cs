using System.Xml.Schema;
using Godot;

[GlobalClass]
public partial class ItemComponent : Node
{
	public InventoryItem Item = new();
	private RigidBody3D _parent;
	[Export] private bool _overwriteSavedPrefab;

	[Export] private Vector2I _itemShape;
	[Export] private string _itemName;
	[Export] private string _itemDescription;
	[Export] private Texture2D _itemSprite;
	[Export] private InventoryItem.EquipmentTypes _itemEquipmentType;
	private InventoryComponent _itemInventory;
	private PackedScene _itemPrefab;

    public override void _Ready()
    {
        _parent = GetParent<RigidBody3D>();
        if (!TryLoadPrefab()) TrySavePrefab();
        if (_overwriteSavedPrefab) TrySavePrefab();
        Init();
    }

    private bool TryLoadPrefab()
    {
    	if (!ResourceLoader.Exists("res://saved/inventoryItemPrefabs/" + _itemName + ".tscn")) return false;
    	var prefab = ResourceLoader.Load<PackedScene>("res://saved/inventoryItemPrefabs/" + _itemName + ".tscn");
    	if (prefab == null) return false;
    	else
    	{
    		_itemPrefab = prefab;
    		return true;
    	}
    }

    private void TrySavePrefab()
    {
    	var scene = new PackedScene();
    	var children = _parent.GetChildren();
    	foreach(Node child in children)
    	{
    		child.Owner = _parent;
    	}
    	var originalTransform = _parent.Transform;
    	_parent.GlobalPosition = Vector3.Zero;
    	_parent.Rotation = Vector3.Zero;
    	_parent.Scale = Vector3.One;
    	scene.Pack(_parent);
    	_parent.Transform = originalTransform;
    	ResourceSaver.Save(scene, "res://saved/inventoryItemPrefabs/" + _itemName + ".tscn");
		GD.PrintErr("Game needs to restart to correctly load saved item prefabs.");
		GetTree().Quit();
    }

	private void Init()
	{
		Item.Shape = _itemShape;
		Item.Name = _itemName;
		Item.Description = _itemDescription;
		Item.Sprite = _itemSprite;
		Item.Prefab = _itemPrefab;
		Item.EquipmentType = _itemEquipmentType;
		_itemInventory = _parent.GetNodeOrNull<InventoryComponent>("InventoryComponent");
		Item.Inventory = _itemInventory;

		if (_itemEquipmentType != InventoryItem.EquipmentTypes.None) Item.Equippable = true;
		if (_itemInventory != null) Item.HasInventory = true;
	}
}
