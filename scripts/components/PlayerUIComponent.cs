using System.Reflection.Metadata.Ecma335;
using Godot;


public partial class PlayerUIComponent : Node
{
    //Node references
    private Node _componentContainer;
    private CharacterBody3D _player;
    private Control _hud;
    private Control _ui;
    private Panel _pauseMenu;
    private Button _resumeButton;
    private Button _settingsButton;
    private Button _quitButton;

    private bool _uiVisible;

    public override void _Ready()
    {
        //Get references
        _componentContainer = GetParent();
        _player = _componentContainer.GetParent<CharacterBody3D>();
        _hud = _player.GetNode<Control>("HUD");
        _ui = _player.GetNode<Control>("UI");
        _pauseMenu = _ui.GetNode<Panel>("PauseMenu");
        _resumeButton = _pauseMenu.GetNode<Button>("ButtonContainer/ResumeButton");
        _settingsButton = _pauseMenu.GetNode<Button>("ButtonContainer/SettingsButton");
        _quitButton = _pauseMenu.GetNode<Button>("ButtonContainer/QuitButton");
        
        //Subscribe to signals
        _resumeButton.Pressed += ResumeButtonPressed;
        _settingsButton.Pressed += SettingsButtonPressed;
        _quitButton.Pressed += QuitButtonPressed;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("inputEscape"))
        {
            switch (_ui.Visible)
            {
                case true: ClosePauseMenu(); break;
                case false: OpenPauseMenu(); break;
            }
        }
    }

    private void OpenPauseMenu()
    {
        _hud.Hide();
        _ui.Show();
        _uiVisible = true;
        Input.MouseMode = Input.MouseModeEnum.Confined;
        GetTree().Paused = true;
    }

    private void ClosePauseMenu()
    {
        _hud.Show();
        _ui.Hide();
        _uiVisible = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GetTree().Paused = false;
    }

    private void ResumeButtonPressed()
    {
        ClosePauseMenu();
    }
    
    private void SettingsButtonPressed()
    {
        //TODO
        return;
    }
    
    private void QuitButtonPressed()
    {
        GetTree().Quit();
    }
}
