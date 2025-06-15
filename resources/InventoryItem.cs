using Godot;

[GlobalClass]
public partial class InventoryItem : Resource
{
    public Vector2I Shape;
    public string Name;
    public string Description;
    public Texture2D Sprite;
    public PackedScene Prefab;
    public bool Equippable;
    public bool Equipped;
    public enum EquipmentTypes
    {
        None,
        EquipmentPrimary,
        EquipmentSecondary,
        EquipmentMelee,
        EquipmentBack,
        EquipmentHead,
        EquipmentChest,
        EquipmentLegs
    };
    public EquipmentTypes EquipmentType;
    public InventoryComponent Inventory;
    public bool HasInventory;
    public bool InInventory;
}
