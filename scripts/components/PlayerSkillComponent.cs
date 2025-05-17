using Godot;
using System;

public partial class PlayerSkillComponent : Node
{
    //Node references
    private Node _componentContainer;
    private PlayerStatsComponent _statsComponent;
    private CharacterBody3D _player;
    private Node3D _head;
    private RayCast3D _headRay;
    private Control _hud;
    private Area3D _aoe;
    private CollisionShape2D _aoeCollision;

    public override void _Ready()
    {
        //Get references
        _componentContainer = GetParent();
        _statsComponent = _componentContainer.GetNode<PlayerStatsComponent>("PlayerStatsComponent");
        _player = _componentContainer.GetParent<CharacterBody3D>();
        _head = _player.GetNode<Node3D>("Head");
        _headRay = _head.GetNode<RayCast3D>("HeadRay");
        _hud = _player.GetNode<Control>("HUD");
        
        //Initialize values
        
    }
    
    
}
