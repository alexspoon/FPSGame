using Godot;

public partial class LevelManager : Node
{
    private Node _main;
    private bool _levelLoaded;
    private Level _currentLevel;
    private Level _levelToLoad;
    [Export] private PackedScene _selectedLevel;

    public override void _Ready()
    {
        _main = GetTree().GetRoot().GetNode<Node>("Main");
        _currentLevel = GetChildOrNull<Level>(0);
        
        if (_currentLevel == null)
        {
            _levelLoaded = false;
            _levelToLoad = _selectedLevel.Instantiate() as Level;
            LoadLevel(_levelToLoad, _levelLoaded);
        }
        else _levelLoaded = true;
    }

    private void LoadLevel(Level level, bool loaded)
    {
        _main.AddChild(level);

        if (!loaded) return;
        
        _main.CallDeferred(Node.MethodName.RemoveChild, _currentLevel);
        _currentLevel = level;
    }
}
