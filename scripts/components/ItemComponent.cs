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
	[Export] private bool _itemEquippable;
	[Export] private InventoryItem.EquipmentTypes _itemEquipmentType;
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

    private bool TrySavePrefab()
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
    	return true;
    }

	private void Init()
	{
		Item.Shape = _itemShape;
		Item.Name = _itemName;
		Item.Description = _itemDescription;
		Item.Sprite = _itemSprite;
		Item.Prefab = _itemPrefab;
		Item.Equippable = _itemEquippable;
		Item.EquipmentType = _itemEquipmentType;
	}
}
