using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
    private CharacterBody3D _player;
    private Level _level;
    private Vector3 _movementTargetPosition = Vector3.Zero;
    public Vector3 MovementTarget
    {
        get { return _navigationAgent.TargetPosition; }
        set { _navigationAgent.TargetPosition = value; }
    }
    private float _movementSpeed = 5;
    private NavigationAgent3D _navigationAgent;
    private CharacterBody3D _playerCharacter;
    public Node ComponentContainer;
    public EnemyHealthComponent HealthComponent;
    public Area3D TorsoHurtBox;
    public Area3D HeadHurtBox;
    private Timer _pathfindTimer;
    private enum _states
    {
        Idle,
        Wander,
        Chase
    };

    public override void _Ready()
    {
        _level = GetParent<Level>();
        _player = _level.GetNode<CharacterBody3D>("Player");
        _pathfindTimer = GetNode<Timer>("PathfindTimer");
        _movementTargetPosition = _player.GlobalPosition;
        ComponentContainer = GetNode<Node>("Components");
        HealthComponent = ComponentContainer.GetNode<EnemyHealthComponent>("EnemyHealthComponent");
        TorsoHurtBox = GetNode<Area3D>("TorsoHurtBox");
        HeadHurtBox = GetNode<Area3D>("HeadHurtBox");
        _navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent");
        
        // These values need to be adjusted for the actor's speed
        // and the navigation layout.
        _navigationAgent.PathDesiredDistance = 0.5f;
        _navigationAgent.TargetDesiredDistance = 0.5f;

        //Subscribe to signals
        _pathfindTimer.Timeout += Pathfind;
        
        // Make sure to not await during _Ready.
        Callable.From(ActorSetup).CallDeferred();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_navigationAgent.IsNavigationFinished())
        {
            return;
        }

        Vector3 currentAgentPosition = GlobalTransform.Origin;
        Vector3 nextPathPosition = _navigationAgent.GetNextPathPosition();

        Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * _movementSpeed;
        MoveAndSlide();
    }

    private void Pathfind()
    {
        Callable.From(ActorSetup).CallDeferred();
    }
    
    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        _movementTargetPosition = _player.GlobalPosition;
        MovementTarget = _movementTargetPosition;
        _pathfindTimer.Start();
    }
    
}
