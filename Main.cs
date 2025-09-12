using Godot;
using Voxel.Chunk;

namespace Voxel;

public partial class Main : Node3D
{
    private long _baseSeed = 123456789;
    private long _worldSeed;
    private long _chunkSeed = 0;

    private static ChunkGenerator _chunkGenerator;
    private static Node3D _chunkParent;

    private static readonly PerlinNoise noiseGenerator = new();

    public static ChunkGenerator ChunkGenerator => _chunkGenerator;

    public static ChunkCoords PlayerCurrentChunk { get; set; }
    public static CharacterBody3D Player { get; set; }

    public const int RenderDistance = 5;

    public static float WorldGenerator(Vector3 position)
    {
        int scale = 4;
        return noiseGenerator.Noise(position.X / scale, position.Z / scale);
    }

    public void GenerateWorldSeed(float x, float y)
    {
        _worldSeed = _baseSeed;
        _worldSeed *= _worldSeed * 6364136223846793005L + 1442695040888963407L;
        _worldSeed *= _worldSeed * 6364136223846793005L + 1442695040888963407L;
        _worldSeed = _baseSeed;
        _worldSeed *= _worldSeed * 6364136223846793005L + 1442695040888963407L;
        _worldSeed *= _worldSeed * 6364136223846793005L + 1442695040888963407L;
    }

    private void _ready()
    {
        var chunkParent = GetNode<Node3D>("Chunks");
        _chunkParent = chunkParent;
        _chunkGenerator = new ChunkGenerator(chunkParent);
        ChunkGenerator.RenderChunk(new ChunkCoords(0, 0));
    }

    private void _process(float delta)
    {
        foreach (var chunk in _chunkGenerator._chunksToRender)
        {
            _chunkParent.AddChild(chunk);
        }
        _chunkGenerator._chunksToRender.Clear();
    }
}
