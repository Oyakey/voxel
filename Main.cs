using Godot;
using Voxel.World;

namespace Voxel;

public partial class Main : Node3D
{
    private static ChunkGenerator _chunkGenerator;

    public static ChunkGenerator ChunkGenerator => _chunkGenerator;
    public static ChunkCoords PlayerCurrentChunk { get; set; }
    public static CharacterBody3D Player { get; set; }

    public static int RenderDistance = 5; //16;

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
    }

    public void _process(float _)
    {
        if (Input.IsActionJustPressed("increase_render_distance"))
        {
            RenderDistance += 1;
        }
        if (Input.IsActionJustPressed("decrease_render_distance"))
        {
            RenderDistance -= 1;
        }
    }
}
