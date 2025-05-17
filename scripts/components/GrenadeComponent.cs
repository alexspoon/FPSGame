using Godot;

public partial class GrenadeComponent : Node
{
    //Node references
    private RigidBody3D _grenade;
    private CollisionShape3D _collision;
    private MeshInstance3D _mesh;
    private Area3D _aoe;
    private CollisionShape3D _aoeCollision;
    private Timer _fuse;
    private float _fuseDuration;
    private float _aoeRange;

    public override void _Ready()
    {
        //Get node references
        _grenade = GetParent<RigidBody3D>();
        _collision = _grenade.GetNode<CollisionShape3D>("Collision");
        _mesh = _grenade.GetNode<MeshInstance3D>("Mesh");
        _aoe = _grenade.GetNode<Area3D>("AOE");
        _aoeCollision = _grenade.GetNode<CollisionShape3D>("AOECollision");
        _fuse = _grenade.GetNode<Timer>("Fuse");
    }
    
}
