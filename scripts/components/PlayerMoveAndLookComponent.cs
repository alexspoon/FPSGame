using Godot;
using System;

public partial class PlayerMoveAndLookComponent : Node
{
    //Node references
    private Node _componentContainer;
    private PlayerHealthComponent _healthComponent;
    private PlayerStatsComponent _statsComponent;
    private CharacterBody3D _player;
    private Node3D _head;
    private Camera3D _camera;
    private Timer _dashTimer;
    private Timer _dashCooldown;
    private Control _hud;
    private Label _speedometer;
    private Timer _slideTimer;
    
    //State variables
    private bool _canDash;
    private bool _isDashing;
    private bool _canMove;
    private bool _isMoving;
    private bool _canSlide;
    private bool _isSliding;
    
    //Move variables
    private float _moveMaxSpeed;
    private float _moveAcceleration;
    private float _moveDrag;
    private float _dashSpeedMultiplier;
    private float _slideSpeedMultiplier;
    private Vector3 _targetVelocity;
    private Vector3 _direction;
    
    //FOV Variables
    [Export] private float _dashFov = 100f;
    [Export] private float _slideFov = 90f;
    [Export] private float _normalFov = 80f;
    [Export] private float _aimFov = 40f;
    
    //Look variables
    [Export] private float _mouseSensitivity = 0.5f;
    
    //Jump variables
    private float _jumpForce;
    private int _maxJumps;
    private float _gravity;
    private int _jumpsLeft;
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            var motion = (InputEventMouseMotion) @event;

            _player.RotateY(-motion.Relative.X * _mouseSensitivity * 0.01f);
            _head.RotateX(-motion.Relative.Y * _mouseSensitivity * 0.01f);

            _head.Rotation = new Vector3((float)Mathf.Clamp(_head.Rotation.X, -Math.PI / 2, Math.PI / 2), _head.Rotation.Y, _head.Rotation.Z);
        }
    }
    
    public override void _Ready()
    {
        //Get references
        _componentContainer = GetParent<Node>();
        _statsComponent = _componentContainer.GetNode<PlayerStatsComponent>("PlayerStatsComponent");
        _healthComponent = _componentContainer.GetNode<PlayerHealthComponent>("PlayerHealthComponent");
        _player = _componentContainer.GetParent<CharacterBody3D>();
        _head = _player.GetNode<Node3D>("Head");
        _camera = _head.GetNode<Camera3D>("FPSCamera");
        _dashTimer = GetNode<Timer>("DashTimer");
        _dashCooldown = GetNode<Timer>("DashCooldown");
        _hud = _player.GetNode<Control>("HUD");
        _speedometer = _hud.GetNode<Label>("Speedometer");
        _slideTimer = GetNode<Timer>("SlideTimer");
        
        //Subscribe to signals
        _dashTimer.Timeout += DashTimerTimeout;
        _dashCooldown.Timeout += DashCooldownTimeout;
        _slideTimer.Timeout += SlideTimerTimeout;
        
        //Initialize values
        MoveAndLookSyncStats();
        ResetJumps();
        _canDash = true;
        _canSlide = true;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public void MoveAndLookSyncStats()
    {
        _moveMaxSpeed = _statsComponent.MoveMaxSpeed;
        _moveAcceleration = _statsComponent.MoveAcceleration;
        _moveDrag = _statsComponent.MoveDrag;
        _jumpForce = _statsComponent.JumpForce;
        _maxJumps = _statsComponent.MaxJumps;
        _gravity = _statsComponent.Gravity;
        _dashTimer.WaitTime = _statsComponent.DashLength;
        _dashCooldown.WaitTime = _statsComponent.DashCooldown;
        _dashSpeedMultiplier = _statsComponent.DashSpeedMultiplier;
        _slideSpeedMultiplier = _statsComponent.SlideSpeedMultiplier;
    }
    
    public override void _Process(double delta)
    {
        if (!_statsComponent.Alive) return;
        Move(delta);
        DashUpdate((float)delta);
    }

    private void Move(double delta)
    {
        _direction = Vector3.Zero;
        var basis = _player.GetNode<CollisionShape3D>("Collision").GlobalTransform.Basis;
        var basisRotation = basis.GetEuler();
            
        if (Input.IsActionPressed("inputD"))
            _direction.X += 1.0f;
        
        if (Input.IsActionPressed("inputA"))
            _direction.X -= 1.0f;
        
        if (Input.IsActionPressed("inputS"))
            _direction.Z += 1.0f;
        
        if (Input.IsActionPressed("inputW"))
            _direction.Z -= 1.0f;
        
        _direction =  _direction.Normalized();
        
        if (_direction != Vector3.Zero)
        {
            basis = Basis.FromEuler(basisRotation);
            _direction = basis * _direction;
            _direction = _direction.Normalized();
            _player.Rotation = basisRotation;
        }

        if (Input.IsActionJustPressed("inputShift") && _dashTimer.IsStopped() && _canDash)
        {
            _moveMaxSpeed *= _dashSpeedMultiplier;
            _moveAcceleration *= _dashSpeedMultiplier;
            _dashTimer.Start();
            _dashCooldown.Start();
            _isDashing = true;
            _canDash = false;
        }

        if (Input.IsActionJustPressed("inputControl") && _slideTimer.IsStopped() && _canSlide && _player.IsOnFloor())
        {
            var headPos = _head.Position;
            headPos.Y -= 1;
            _head.Position = headPos;
            _moveMaxSpeed *= _slideSpeedMultiplier;
            _moveAcceleration *= _slideSpeedMultiplier;
            _slideTimer.Start();
            _isSliding = true;
            _canSlide = false;
        }

        var targetVelocityWithoutY = new Vector3(_targetVelocity.X, 0, _targetVelocity.Z);
        var dragForce = -targetVelocityWithoutY * _moveDrag * (float)delta;

        _targetVelocity += dragForce;

        var deltaVelocity = _direction * _moveAcceleration * (float)delta;
        deltaVelocity = deltaVelocity.Normalized() * MathF.Min(deltaVelocity.Length(),
            MathF.Max(0, _moveMaxSpeed - targetVelocityWithoutY.Dot(deltaVelocity.Normalized())));
        _targetVelocity += deltaVelocity;
        
        if (_player.IsOnFloor())
        {
            ResetJumps();
        }
        
        if (!_player.IsOnFloor())
        {
            _targetVelocity.Y -= _gravity * (float)delta * 2;
        }
        
        if (_jumpsLeft > 0 && Input.IsActionJustPressed("inputSpace"))
        {
            _jumpsLeft --;
            _targetVelocity.Y = _jumpForce;
        }
        
        _player.Velocity = _targetVelocity;
        _player.MoveAndSlide();
        _speedometer.Text = "" + Mathf.Round(_player.Velocity.Length());
    }

    private void DashTimerTimeout()
    {
        _isDashing = false;
        _moveMaxSpeed /= _dashSpeedMultiplier;
        _moveAcceleration /= _dashSpeedMultiplier;
    }

    private void DashCooldownTimeout()
    {
        _canDash = true;
    }
    
    private void DashUpdate(float delta)
    {
        if (_isDashing && _direction != Vector3.Zero)
        {
            _camera.Fov = Mathf.Lerp(_camera.Fov, _dashFov, 5f * delta);
        }
        else _camera.Fov = Mathf.Lerp(_camera.Fov, _normalFov, 3f * delta);
    }

    private void ResetJumps()
    {
        _jumpsLeft = _maxJumps;
    }

    private void SlideTimerTimeout()
    {
        _isSliding = false;
        _canSlide = true;
        _moveMaxSpeed /= _slideSpeedMultiplier;
        _moveAcceleration /= _slideSpeedMultiplier;
        var headPos = _head.Position;
        headPos.Y += 1;
        _head.Position = headPos;
    }
    
    private void SlideUpdate(float delta)
    {
        if (_isSliding && _direction != Vector3.Zero)
        {
            _camera.Fov = Mathf.Lerp(_camera.Fov, _slideFov, 5f * delta);
        }
        else _camera.Fov = Mathf.Lerp(_camera.Fov, _normalFov, 3f * delta);
    }
}
