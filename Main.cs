using Godot;
using Voxel.World;

namespace Voxel;

public partial class Main : Node3D
{
    private static ChunkGenerator _chunkGenerator;

    public static ChunkGenerator ChunkGenerator => _chunkGenerator;
    public static ChunkCoords PlayerCurrentChunk { get; set; }
    public static CharacterBody3D Player { get; set; }

    public const int RenderDistance = 5;

    private void _ready()
    {
        var chunkParent = GetNode<Node3D>("Chunks");
        var rerenderQueue = new RerenderQueue();
        _chunkGenerator = new ChunkGenerator(
            chunkParent,
            RenderDistance,
            new ChunkCache(rerenderQueue),
            rerenderQueue
        );
        // var worldCoords = new BlockCoords(3, 16, 3);
        // GD.Print(Mathf.FloorToInt(-1.0 / 16.0));
        // GD.Print(Mathf.FloorToInt(-0.5));
        // GD.Print(Mathf.FloorToInt(0.5));
        // GD.Print(Mathf.FloorToInt(1 / 16));
        // var chunkCoords = ChunkData.ChunkCoordsFromWorldBlockCoords(worldCoords);
        // GD.Print(chunkCoords);
        // GD.Print(ChunkData.WorldBlockCoordsToLocalBlockCoords(worldCoords, chunkCoords));
    }
}
