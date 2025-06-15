using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class InventoryComponent : Node
{
    private int _sizeX = 5;
    private int _sizeY = 5;

    private int[,] _grid;
    private GridContainer _hudGrid;
    private List<InventoryItem> _items;
 
    public override void _Ready()
    {
        _grid = new int[_sizeX, _sizeY];
        _hudGrid = GetNode<GridContainer>("HUDPanel/InventoryGrid");
        
    }
    
  
}
