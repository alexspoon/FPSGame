using Godot;
using System;

public partial class PlayerStatsComponent : Node
{
    //Component references
    private PlayerMoveAndLookComponent _moveAndLookComponent;
    private PlayerHealthComponent _healthComponent;
    private Node _componentContainer;
    private CharacterBody3D _player;
    private Control _hud;
    private ProgressBar _healthBar;

    //Stat variables
    public bool Alive = true;
    [Export] public float MaxHealth;
    [Export] public float MoveMaxSpeed;
    [Export] public float MoveAcceleration;
    [Export] public float MoveDrag;
    [Export] public float JumpForce;
    [Export] public int MaxJumps;
    [Export] public float Gravity;
    
    public override void _Ready()
    {
        _componentContainer = GetParent<Node>();
        _player = _componentContainer.GetParent<CharacterBody3D>();
        _hud = _player.GetNode<Control>("HUD");
        _healthBar = _hud.GetNode<ProgressBar>("HealthBar");
        _moveAndLookComponent = _componentContainer.GetNode<PlayerMoveAndLookComponent>("PlayerMoveAndLookComponent");
        _healthComponent = _componentContainer.GetNode<PlayerHealthComponent>("PlayerHealthComponent");
    }
    
    public void MaxHealthIncrease(int amount)
    {
        Mathf.Round(MaxHealth);
        MaxHealth += amount;
        _healthBar.MaxValue = MaxHealth;
    }
    
    public void MaxHealthDecrease(int amount)
    {
        Mathf.Round(MaxHealth);
        MaxHealth -= amount;
        _healthBar.MaxValue = MaxHealth;
    }
}
