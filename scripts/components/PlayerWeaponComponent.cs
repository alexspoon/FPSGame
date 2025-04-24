using Godot;

public partial class PlayerWeaponComponent : Node
{
    //Node references
    private Node _componentContainer;
    private PlayerStatsComponent _statsComponent;
    private CharacterBody3D _player;
    private Node3D _head;
    private RayCast3D _headRay;
    private Timer _fireRateTimer;
    private Timer _reloadTimer;
    private Control _hud;
    private Label _ammoCounter;
    [Export] private PackedScene _projectile;
    
    private float _damage;
    private float _range;
    private float _velocity;
    private float _fireRate;
    private float _reloadTime;
    private int _ammoCount;
    private int _ammoLoaded;
    private int _magazineSize;
    private int _shotCount;
    
    public override void _Ready()
    {
        //Get references
        _componentContainer = GetParent();
        _statsComponent = _componentContainer.GetNode<PlayerStatsComponent>("PlayerStatsComponent");
        _player = _componentContainer.GetParent<CharacterBody3D>();
        _head = _player.GetNode<Node3D>("Head");
        _headRay = _head.GetNode<RayCast3D>("HeadRay");
        _fireRateTimer = GetNode<Timer>("FireRateTimer");
        _reloadTimer = GetNode<Timer>("ReloadTimer");
        _hud = _player.GetNode<Control>("HUD");
        _ammoCounter = _hud.GetNode<Label>("AmmoCounter");

        //Subscribe to signals
        _reloadTimer.Timeout += Reload;
        
        //Initialize values
        WeaponSyncStats();
        _ammoLoaded = _magazineSize;
        _ammoCounter.Text = _ammoLoaded + " / " + _ammoCount;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("inputLeftMouse"))
        {
            Fire();   
        }

        if (Input.IsActionJustPressed("inputR"))
        {
            InitiateReload();
        }
    }

    public void WeaponSyncStats()
    {
        _damage = _statsComponent.WeaponDamage;
        _fireRate = _statsComponent.WeaponFireRate;
        _fireRateTimer.WaitTime = _fireRate;
        _reloadTime = _statsComponent.WeaponReloadTime;
        _reloadTimer.WaitTime = _reloadTime;
        _ammoCount = _statsComponent.WeaponAmmoCount;
        _magazineSize = _statsComponent.WeaponMagazineSize;
        _shotCount = _statsComponent.WeaponShotCount;
        _velocity = _statsComponent.WeaponVelocity;
        _range = _statsComponent.WeaponRange;
        _headRay.TargetPosition = new Vector3(0, 0, -_range);
    }

    private void Fire()
    {
        if (_ammoLoaded == 0)
        {
            GD.Print("No ammo loaded");
            return;
        }
        if (!_reloadTimer.IsStopped()) return;
        if (!_fireRateTimer.IsStopped()) return;

        if (_headRay.IsColliding() && _headRay.GetCollider() is Enemy)
        {
            var enemy = _headRay.GetCollider() as Enemy;
            enemy.HealthComponent.Damage(_damage);
        }
        _fireRateTimer.Start();
        _ammoLoaded -= _shotCount;
        _ammoCounter.Text = _ammoLoaded + " / " + _ammoCount;
        GD.Print("Weapon fired");
        GD.Print("Ammo loaded: " + _ammoLoaded);
    }

    private void InitiateReload()
    {
        if (_ammoLoaded == _magazineSize) return;
        if (!_reloadTimer.IsStopped()) return;
        _reloadTimer.Start();
        GD.Print("Reload initiated");
    }

    private void Reload()
    {
        var ammoToLoad = _magazineSize - _ammoLoaded;
        _ammoCount -= ammoToLoad;
        _ammoLoaded += ammoToLoad;
        _ammoCounter.Text = _ammoLoaded + " / " + _ammoCount;
        GD.Print("Reloaded " + ammoToLoad + " bullets");
    }
}
