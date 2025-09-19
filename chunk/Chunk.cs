using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Voxel.Blocks;

namespace Voxel.Chunk;

public partial class Chunk : MeshInstance3D
{
    public const int SIDE_LENGTH = 16;
    public const int MIN_HEIGHT = -64;
    public const int MAX_HEIGHT = 192;

    private long _worldSeed = 123456789;
    private long _chunkSeed;
    private CollisionShape3D _collisionShape;

    private MeshRenderer _meshRenderer = new();
    private ChunkData _chunkData;

    private static readonly PackedScene chunkPrefab = ResourceLoader
      .Load<PackedScene>("res://chunk/chunk.tscn");

    private Mesh _tempMesh;

    private bool _isRendering = false;

    public static Chunk Spawn(ChunkData chunkData)
    {
        var chunk = chunkPrefab.Instantiate<Chunk>();
        chunk._chunkData = chunkData;
        var x = chunkData.Coords.X;
        var y = chunkData.Coords.Y;
        chunk.Position = new Vector3(x * SIDE_LENGTH, 0, y * SIDE_LENGTH);
        // chunk.renderBlocks();
        return chunk;
    }

    public void _ready()
    {
        RenderBlocksOnThread();
        _collisionShape = GetNode<CollisionShape3D>("StaticBody3D/CollisionShape3D");
    }

    public void Rerender()
    {
        if (_isRendering)
            return;

        _meshRenderer = new MeshRenderer();
        RenderBlocksOnThread();
    }

    private void _process(float _)
    {
        AddMeshToScene();

        var chunkCoords = Main.PlayerCurrentChunk;

        var distanceXFromPlayer = Math.Abs(_chunkData.Coords.X - chunkCoords.X);
        var distanceYFromPlayer = Math.Abs(_chunkData.Coords.Y - chunkCoords.Y);

        if (distanceXFromPlayer > Main.RenderDistance || distanceYFromPlayer > Main.RenderDistance)
        {
            Main.ChunkGenerator.RemoveChunk(_chunkData.Coords);
        }
    }

    private void RenderBlocksOnThread()
    {
        new Task(RenderBlocks).Start();
    }

    private void RenderBlocksAsync()
    {
        new Thread(RenderBlocks).Start();
    }

    private void AddMeshToScene()
    {
        if (_tempMesh == null)
            return;
        Mesh = _tempMesh;
        AddCollider();
        _tempMesh = null;
    }

    private void AddCollider()
    {
        if (_tempMesh == null)
            return;
        var shape = new ConcavePolygonShape3D();
        shape.SetFaces(Mesh.GetFaces());
        _collisionShape.Shape = shape;
    }
    public void BreakBlock(BlockCoords coords)
    {
        if (!_chunkData.CanBreakBlock(coords))
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

    private void RenderBlocks()
    {
        // We need to check what blocks should be rendered
        // They are :
        // 1. Blocks that are in the chunk (in _blocks)
        // 2. Blocks that are NOT surrounded by blocks on all sides
        // 3. Theses blocks should have only their visible faces rendered
        // We also need to make this operation asynchronous so that it won't freeze the game
        _isRendering = true;

        for (var x = 0; x < SIDE_LENGTH; x++)
        {
            for (var z = 0; z < SIDE_LENGTH; z++)
            {
                for (var y = MIN_HEIGHT; y < MAX_HEIGHT; y++)
                {
                    int renderMode = 0;

                    var block = _chunkData.GetBlock(new BlockCoords(x, y, z));

                    if (block == null)
                    {
                        continue;
                    }

                    BlockData eastBlock = _chunkData.GetBlock(new BlockCoords(x + 1, y, z));
                    BlockData westBlock = _chunkData.GetBlock(new BlockCoords(x - 1, y, z));
                    BlockData upBlock = _chunkData.GetBlock(new BlockCoords(x, y + 1, z));
                    BlockData downBlock = _chunkData.GetBlock(new BlockCoords(x, y - 1, z));
                    BlockData southBlock = _chunkData.GetBlock(new BlockCoords(x, y, z + 1));
                    BlockData northBlock = _chunkData.GetBlock(new BlockCoords(x, y, z - 1));

                    if (!IsBlockOpaque(eastBlock))
                        renderMode += (int)BlockDirection.East;
                    if (!IsBlockOpaque(westBlock))
                        renderMode += (int)BlockDirection.West;
                    if (!IsBlockOpaque(upBlock))
                        renderMode += (int)BlockDirection.Up;
                    if (!IsBlockOpaque(downBlock))
                        renderMode += (int)BlockDirection.Down;
                    if (!IsBlockOpaque(southBlock))
                        renderMode += (int)BlockDirection.South;
                    if (!IsBlockOpaque(northBlock))
                        renderMode += (int)BlockDirection.North;

                    AddFace(new BlockCoords(x, y, z), block, renderMode);
                }
            }
        }

        var arrMesh = new ArrayMesh();
        var surfaceArray = _meshRenderer.GetSurfaceArray();

        // No blendshapes, lods, or compression used.
        arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);

        StandardMaterial3D atlasMaterial = new()
        {
            AlbedoTexture = GD.Load<Texture2D>("res://resources/images/textures-atlas.png"),
            CullMode = BaseMaterial3D.CullModeEnum.Back, // optional: controls backface culling
            TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
            // Big performance gains when using this instead of PerPixel.
            ShadingMode = BaseMaterial3D.ShadingModeEnum.PerVertex,
            // DisableReceiveShadows = true,
        };

        arrMesh.SurfaceSetMaterial(0, atlasMaterial);

        _tempMesh = arrMesh;
        _isRendering = false;
    }

    // This old system will be removed once mesh generation is finished.
    // It should increase performance by a lot. And allow mesh generation to be done in a separate thread.
    // The tradeoff is that we cannot use godot nodes anymore.
    private void AddFace(BlockCoords coords, BlockData block, int renderMode)
    {
        if (renderMode == 0)
            return;

        if (block.Type == BlockType.Air)
            return;

        var blockPosition = new Vector3(coords.X, coords.Y, coords.Z);

        if ((renderMode & (int)BlockDirection.East) != 0)
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.East);
        if ((renderMode & (int)BlockDirection.West) != 0)
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.West);
        if ((renderMode & (int)BlockDirection.Up) != 0)
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.Up);
        if ((renderMode & (int)BlockDirection.Down) != 0)
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.Down);
        if ((renderMode & (int)BlockDirection.South) != 0)
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.South);
        if ((renderMode & (int)BlockDirection.North) != 0)
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.North);
    }

    private static bool IsBlockOpaque(BlockData blockData)
    {
        // Hacky workaround to avoid rendering blocks at the bottom of the chunk.
        // Remove when occlusion culling is implemented.
        if (blockData == null)
            return true;

        return blockData != null && blockData.Type != BlockType.Air;
    }

    private static Vector2[] GetUVsFromAtlas(Vector2I tileCoords, Vector2I tilesInAtlas)
    {
        float tileWidth = 1f / tilesInAtlas.X;
        float tileHeight = 1f / tilesInAtlas.Y;
        Vector2 offset = new Vector2(tileCoords.X * tileWidth, tileCoords.Y * tileHeight);

        return [
            offset + new Vector2(0, tileHeight),           // Bottom-left
            offset + new Vector2(tileWidth, tileHeight),   // Bottom-right
            offset + new Vector2(tileWidth, 0),            // Top-right
            offset + new Vector2(0, 0)                     // Top-left
        ];
    }
}
