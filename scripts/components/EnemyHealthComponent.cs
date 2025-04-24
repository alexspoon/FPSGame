using Godot;


public partial class EnemyHealthComponent : Node
{
    private Node _componentContainer;
    private CharacterBody3D _enemy;
    private ProgressBar _healthBar;
    
    private float _currentHealth;
    private float _maxHealth;
    private float _armor;
    private bool _alive;

    public override void _Ready()
    {
        //Get references
        _componentContainer = GetParent();
        _enemy = _componentContainer.GetParent<CharacterBody3D>();
        _healthBar = _enemy.GetNode<ProgressBar>("HealthBarViewport/HealthBar");
        
        //test
        _maxHealth = 100;
        _armor = 0;
        
        //Initialize values
        _alive = true;
        _currentHealth = _maxHealth;
        _healthBar.MaxValue = _maxHealth;
        _healthBar.Value = _currentHealth;
    }

    private void Kill()
    {
        _alive = false;
        _enemy.QueueFree();
    }
    
    public void EnemySyncStats()
    {
        
    }

    public void Damage(float damage)
    {
        _currentHealth -= damage - _armor;
        _healthBar.Value = _currentHealth;
        if (_currentHealth <= 0)
        {
            Kill();
        }
    }
}
