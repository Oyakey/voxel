using Godot;
using Voxel.Chunk;

namespace Voxel.Managers;

public partial class GameManager : Node3D
{
    private static ChunkGenerator _chunkGenerator;
    private static Node3D _chunkParent;
    private static readonly PerlinNoise noiseGenerator = new();

    public static ChunkGenerator ChunkGenerator => _chunkGenerator;
    public static ChunkCoords PlayerCurrentChunk { get; set; }
    public static CharacterBody3D Player { get; set; }

    public const WorldGenerator WorldGenerator = new();
    public const int RenderDistance = 5;


    private static void _ready()
    {
        var chunkParent = GetNode<Node3D>("Chunks");
        WorldGenerator.Init(chunkParent);
    }

    private void _process(float _)
    {
        foreach (var chunk in _chunkGenerator._chunksToRender)
        {
            _chunkParent.AddChild(chunk);
        }
        _chunkGenerator._chunksToRender.Clear();
    }
}
