using Godot;
using System;
using System.Threading.Tasks;

public partial class ProceduralGridmap : GridMap
{
    [Signal] private delegate void GenerationCompleteEventHandler();
    [Export] private NoiseTexture3D _proceduralNoise;
    [Export] private uint _noiseSeed;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();
    private Noise _generatedNoise;
    [Export] private int _width;
    [Export] private int _height;
    [Export] private int _depth;

    public override void _Ready()
    {
        _proceduralNoise.Width = _width;
        _proceduralNoise.Height = _height;
        _proceduralNoise.Depth = _depth;
        _generatedNoise = _proceduralNoise.Noise;
        AsyncTaskManager();
        // Generate();
        // var debug = MeshLibrary.FindItemByName("Stone");
        // GD.Print(debug);
    }

    public override void _Process(double delta)
    {
        // if (Input.IsActionJustPressed("inputE")) Generate();
    }

    private async Task AsyncTaskManager()
    {
        Task<int> generateTask = Generate();
        var result = await generateTask;
        GD.Print(result);
    }
    
    private async Task<int> Generate()
    {
        await Task.Delay(1000);
        Clear();
        _rng.Randomize();
        _noiseSeed = _rng.Randi();
        _generatedNoise.Set("seed", _noiseSeed);
        _generatedNoise.Set("width", _width);
        _generatedNoise.Set("height", _height);
        _generatedNoise.Set("depth", _depth);

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int z = 0; z < _depth; z++)
                {
                    var noiseValue = _generatedNoise.GetNoise3D(x, y, z);
                    if (noiseValue > 0.1)
                    {
                        SetCellItem(new Vector3I(x, y, z), 0);
                    }
                }
            }
        }
        //When done,
        EmitSignalGenerationComplete();
        return 1;
    }
}
