using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class InventoryComponent : Node
{
    [Export] public int Width;
    [Export] public int Height;
    public bool IsOpen;
    public int[,] Grid;
    public List<InventoryItem> Items;

    public override void _Ready()
    {
        Grid = new int[Width, Height];
    }
}
