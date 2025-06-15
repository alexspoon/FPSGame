using Godot;

public partial class LevelManager : Node
{
    private Node _main;
    private bool _levelLoaded;
    private Level _currentLevel;
    private Level _levelToLoad;
    private SubViewportContainer _pixelViewportContainer;
    private SubViewport _pixelViewport;
    [Export] private PackedScene _selectedLevel;
    [Export ]private bool _pixelationEnabled;

    public override void _Ready()
    {
        _main = GetTree().GetRoot().GetNode<Node>("Main");
        _pixelViewportContainer = _main.GetNode<SubViewportContainer>("PixelShaderContainer");
        _pixelViewport = _pixelViewportContainer.GetNode<SubViewport>("PixelViewport");
        _currentLevel = GetChildOrNull<Level>(0);
        _pixelViewportContainer.Hide();
        
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
        var loader = _main;
        if (_pixelationEnabled)
        {
            _pixelViewportContainer.Show();
            loader = _pixelViewport;
        }
        
        loader.AddChild(level);

        if (!loaded) return;
        
        loader.CallDeferred(Node.MethodName.RemoveChild, _currentLevel);
        _currentLevel = level;
    }
}
