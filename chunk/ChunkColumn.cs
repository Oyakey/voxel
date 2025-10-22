using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Voxel.Blocks;

namespace Voxel.Chunk;

public partial class ChunkColumn : Node3D
{
    public const int SIDE_LENGTH = 16;
    public const int MIN_HEIGHT = -64;
    public const int MAX_HEIGHT = 192;
    public static int BUILD_HEIGHT => MAX_HEIGHT - MIN_HEIGHT;
    public static int SLICES_COUNT => BUILD_HEIGHT / SIDE_LENGTH;

    private long _worldSeed = 123456789;
    private long _chunkSeed;
    private CollisionShape3D _collisionShape;
    private MeshRenderer _meshRenderer = new();
    private ChunkData _chunkData;
    // Slices nodes and ChunkData
    private readonly ChunkData[] _slicesData = new ChunkData[SLICES_COUNT];
    private ChunkSlice[] _chunkSlices;

    private static readonly PackedScene chunkPrefab = ResourceLoader
      .Load<PackedScene>("res://chunk/chunk.tscn");

    private Mesh _tempMesh;

    private bool _isRendering = false;

    public static ChunkColumn Spawn(ChunkData chunkData)
    {
        var column = chunkPrefab.Instantiate<ChunkColumn>();
        column._chunkData = chunkData;
        var x = chunkData.Coords.X;
        var y = chunkData.Coords.Y;
        column.Position = new Vector3(x * SIDE_LENGTH, 0, y * SIDE_LENGTH);
        return column;
    }

    public void _ready()
    {
        _collisionShape = GetNode<CollisionShape3D>("StaticBody3D/CollisionShape3D");

        initializeSlices();
    }

    private void initializeSlices()
    {
        _chunkSlices = new ChunkSlice[SLICES_COUNT];
        for (var i = 0; i < SLICES_COUNT; i++)
        {
            var slice = new ChunkSlice(i, _chunkData);
            _chunkSlices[i] = slice;
            AddChild(slice);
        }
    }

    public void Rerender()
    {
        if (_isRendering)
            return;

        _meshRenderer = new MeshRenderer();
        // RenderBlocksOnThread();
    }

    private void _process(float _)
    {
        var chunkCoords = Main.PlayerCurrentChunk;

        var distanceXFromPlayer = Math.Abs(_chunkData.Coords.X - chunkCoords.X);
        var distanceYFromPlayer = Math.Abs(_chunkData.Coords.Y - chunkCoords.Y);

        if (distanceXFromPlayer > Main.RenderDistance || distanceYFromPlayer > Main.RenderDistance)
        {
            Main.ChunkGenerator.RemoveChunk(new(_chunkData.Coords.X, _chunkData.Coords.Z));
        }
    }

    public void BreakBlock(BlockCoords coords)
    {
        var chunkData = _slicesData[coords.Y / SIDE_LENGTH];
        if (!chunkData.CanBreakBlock(coords))
        {
            GD.Print($"Can't break block at {coords}");
            return;
        }
        GD.Print($"Broke at {coords}");
        _chunkData.Break(coords);
        Rerender();
    }

    public void PlaceBlock(BlockCoords coords)
    {
        _chunkData.Place(coords);
        Rerender();
    }

}
