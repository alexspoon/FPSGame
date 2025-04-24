using Godot;

public partial class Projectile : Node3D
{
    //Node references
    private MeshInstance3D _mesh;
    private Area3D _area;
    private CollisionShape3D _collider;
    private RayCast3D _collisionRay;
    
    //Physics variables
    private Vector3 _velocity;
    public float SpawnVelocity;
    private float _drag;
    private float _gravity;
    
    //Damage variables
    public float Damage;
    
    public override void _Ready()
    {
        //Get references
        _mesh = GetNode<MeshInstance3D>("Mesh");
        _collisionRay = GetNode<RayCast3D>("CollisionRay");
        
        //Initialize values
        _drag = 0.1f;
        _velocity = new Vector3(0, 0, 0);
        _velocity.Z = -SpawnVelocity;
        _gravity = 9.82f;
    }

    public override void _Process(double delta)
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyBasicForces((float)delta);
        GD.Print("bullet is alive!!!!!");
        CheckCollision((float) delta);
    }

    private void ApplyVelocity(float delta)
    {
        var transform = GlobalTransform;
        transform.Origin += _velocity * delta; 
        GlobalTransform = transform;
    }

    private void ApplyDrag(float delta)
    {
       var velocityWithoutY = new Vector3(_velocity.X, 0, _velocity.Z);
       var dragForce = -velocityWithoutY * _drag * delta;
       _velocity += dragForce;
    }

    private void ApplyGravity(float delta)
    {
        _velocity.Y -= _gravity * delta;
    }

    private void ApplyBasicForces(float delta)
    {
        ApplyVelocity(delta);
        ApplyDrag(delta); 
        ApplyGravity(delta);
    }

    private void CheckCollision(float delta)
    {
        _collisionRay.SetTargetPosition(_velocity * delta);
        LookAt(_collisionRay.TargetPosition);
        if (!_collisionRay.IsColliding()) return;
        GD.Print("collision");
        QueueFree();
    }
 
}
