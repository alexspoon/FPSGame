using Godot;

public partial class ContextMenu : Panel
{
    public InventoryItem TargetItem;
	private BoxContainer _buttonContainer;
	private Button _equipButton;
	private Button _dropButton;
    private Control _hud;
    private CharacterBody3D _player;
    private Node _componentContainer;
    private PlayerEquipmentComponent _equipmentComponent;


    public override void _Ready()
    {
    	GetReferences();
        HideMenu();

    	_equipButton.Pressed += Equip;
    	_dropButton.Pressed += Drop;
    }

    private void GetReferences()
    {
        _equipButton = GetNode<Button>("ButtonContainer/EquipButton");
        _dropButton = GetNode<Button>("ButtonContainer/DropButton");
        _hud = GetParent<Control>();
        _player = _hud.GetParent<CharacterBody3D>();
        _componentContainer = _player.GetNode<Node>("Components");
        _equipmentComponent = _componentContainer.GetNode<PlayerEquipmentComponent>("PlayerEquipmentComponent");
    }

    public void RevealMenu()
    {
        GlobalPosition = GetGlobalMousePosition();
        Visible = true;
    }

    public void HideMenu()
    {
        GlobalPosition = Vector2.Zero;
        Visible = false;
        TargetItem = null;
        _equipButton.Disabled = true;
        _dropButton.Disabled = true;
    }

    public void SelectItem(InventoryItem selectedItem)
    {
        RevealMenu();
        if (selectedItem.Equippable) _equipButton.Disabled = false;
        if (selectedItem.Equipped) _equipButton.Disabled = true;
        _dropButton.Disabled = false;
        TargetItem = selectedItem;
    }

    public void Equip()
    {
        _equipmentComponent.TryEquipItem(TargetItem);
        HideMenu();
    }

    public void Drop()
    {
        _equipmentComponent.DropItem(TargetItem);
        HideMenu();
    }
}
