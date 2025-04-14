using Godot;

public partial class PlayerHealthComponent : Node
{
    //Node references
    private Node _componentContainer;
    private PlayerStatsComponent _statsComponent;
    private CharacterBody3D _player;
    private Control _hud;
    private ProgressBar _healthBar;
    
    //Bool variables
    private bool _invincible;
    
    //Health variables
    private float _maxHealth; 
    private float _currentHealth;

    public override void _Ready()
    {
        //Get references
        _componentContainer = GetParent();
        _statsComponent = _componentContainer.GetNode<PlayerStatsComponent>("PlayerStatsComponent");
        _player = _componentContainer.GetParent<CharacterBody3D>();
        _hud = _player.GetNode<Control>("HUD");
        _healthBar = _hud.GetNode<ProgressBar>("HealthBar");
        
        //Initialize values
        HealthSyncStats();
        _currentHealth = _maxHealth;
        _healthBar.Value = _currentHealth;
        _healthBar.MaxValue = _maxHealth;
    }

    public void HealthSyncStats()
    {
        _maxHealth = _statsComponent.MaxHealth;
    }
    
    public void Damage(float amount)
    {
        HealthSyncStats();
        Mathf.Round(_currentHealth);
        if (!_statsComponent.Alive) return;
        if (_invincible) return;
        _currentHealth -= amount;
        _healthBar.Value = _currentHealth;
        if (_currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Heal(float amount)
    {
        HealthSyncStats();
        Mathf.Round(_currentHealth);
        Mathf.Round(_maxHealth);
        if (!_statsComponent.Alive) return;
        if (_currentHealth == _maxHealth) return;
        if (_currentHealth + amount >= _maxHealth)
        {
            _currentHealth = _maxHealth;
            _healthBar.Value = _currentHealth;
            return;
        }
        _currentHealth += amount;
        _healthBar.Value = _currentHealth;
    }

    public void Kill()
    {
        _statsComponent.Alive = false;
        _player.QueueFree();
    }
}
