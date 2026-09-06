using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Voxel.Blocks;
using Voxel.Chunks;

namespace Voxel.World;

public partial class Chunk : MeshInstance3D
{
    public const int MIN_HEIGHT = -64;
    public const int MAX_HEIGHT = 192;
    public const int Y_CHUNKS = 16;
    public const int LOWEST_CHUNK = -4;
    public const int HIGHEST_CHUNK = 12;
    public const int CHUNK_LENGTH = 16;
    public const int BLOCKS_PER_CHUNK = 4096;

    private long _worldSeed = 123456789;
    private long _chunkSeed;
    private CollisionShape3D _collisionShape;

    // TODO: Make this modifiable when spawning the chunk and implement behavior for the generator.
    private ushort _lod = 1; // Should be a multiple of 2. 0 will crash the game.

    private MeshRenderer _meshRenderer;
    private ChunkCoords _chunkCoords;
    private ChunkCache _chunkCache;
    private RerenderQueue _rerenderQueue;

    private static readonly PackedScene chunkPrefab = ResourceLoader
      .Load<PackedScene>("res://chunks/chunk.tscn");

    private Mesh _tempMesh;

    private bool _isRendering = false;
    private bool _hasRendered = false;
    private ulong _lastRerenderTime = 0;

    public static Chunk Spawn(ChunkData chunkData, ChunkCache chunkCache, RerenderQueue rerenderQueue)
    {
        var chunk = chunkPrefab.Instantiate<Chunk>();
        chunk._chunkCoords = chunkData.Coords;
        chunk._chunkCache = chunkCache;
        chunk._rerenderQueue = rerenderQueue;
        var x = chunkData.Coords.X;
        var y = chunkData.Coords.Y;
        var z = chunkData.Coords.Z;
        chunk.Position = new Vector3(x * CHUNK_LENGTH, y * CHUNK_LENGTH, z * CHUNK_LENGTH);
        return chunk;
    }

    public void _ready()
    {
        // AddChild(new CsgBox3D());
        _collisionShape = GetNode<CollisionShape3D>("StaticBody3D/CollisionShape3D");
    }

    public bool CanRender()
    {
        // If the chunk is already being rendered, we don't need to render it again
        // If the chunk is not in the cache, it has not been generated yet.
        return !_hasRendered && !_isRendering && _chunkCache.ContainsChunk(_chunkCoords);
    }

    public void Rerender()
    {
        RenderBlocksTask();
    }

    private void _process(float _)
    {
        ListenToRerenderQueue();
        RenderChunk();
        AddMeshToScene();
        RemoveChunkIfNecessary();
    }

    private void ListenToRerenderQueue()
    {
        if (_rerenderQueue.ShouldRender(_chunkCoords))
        {
            var queueRerenderTime = _rerenderQueue.GetRerenderTime(_chunkCoords);
            if (queueRerenderTime != null && queueRerenderTime > _lastRerenderTime)
            {
                _hasRendered = false;
            }
        }
    }

    public void RenderChunk()
    {
        if (!CanRender())
            return;

        _isRendering = true;
        _meshRenderer = new MeshRenderer();

        RenderBlocksTask();
        // RenderBlocks();
    }

    private void RenderBlocksTask()
    {
        Task.Run(RenderBlocks);
    }

    private void RenderBlocksThread()
    {
        new Thread(RenderBlocks).Start();
    }

    private ChunkData GetChunkData()
    {
        if (!_chunkCache.ContainsChunk(_chunkCoords))
            return null;
        return _chunkCache.GetChunk(_chunkCoords);
    }

    private void RemoveChunkIfNecessary()
    {
        if (GetChunkData() == null)
            return;

        var playerChunkCoords = Main.PlayerCurrentChunk;

        var distanceXFromPlayer = Math.Abs(GetChunkData().Coords.X - playerChunkCoords.X);
        // var distanceYFromPlayer = Math.Abs(_chunkData.Coords.Y - playerChunkCoords.Y);
        var distanceZFromPlayer = Math.Abs(GetChunkData().Coords.Z - playerChunkCoords.Z);

        // if (distanceXFromPlayer > Main.RenderDistance || distanceZFromPlayer > Main.RenderDistance || distanceYFromPlayer > Main.RenderDistance)
        if (distanceXFromPlayer > Main.RenderDistance || distanceZFromPlayer > Main.RenderDistance)
        {
            Main.ChunkGenerator.RemoveChunk(GetChunkData().Coords);
        }
    }

    private void AddMeshToScene()
    {
        if (_tempMesh == null || GetChunkData() == null)
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
        if (!GetChunkData().CanBreakBlock(coords))
        {
            GD.Print($"Can't break block at {coords}");
            return;
        }
        GD.Print($"Broke at {coords}");
        GetChunkData().Break(coords);
        Rerender();
    }

    public void PlaceBlock(BlockCoords coords)
    {
        GetChunkData().Place(coords);
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

        var hasAddedAFace = false;

        for (var x = 0; x < CHUNK_LENGTH; x += _lod)
        {
            for (var z = 0; z < CHUNK_LENGTH; z += _lod)
            {
                for (var y = 0; y < CHUNK_LENGTH; y += _lod)
                {
                    int renderMode = 0;

                    var block = GetBlock(new BlockCoords(x, y, z));

                    if (block == null)
                    {
                        // GD.PushWarning($"Block at {x}, {y}, {z} is null, this should not happen");
                        throw new Exception($"Block at {x}, {y}, {z} is null, this should not happen");
                    }

                    if (!IsBlockOpaque(block))
                    {
                        continue;
                    }

                    BlockData eastBlock = GetBlock(new BlockCoords(x + _lod, y, z));
                    BlockData westBlock = GetBlock(new BlockCoords(x - _lod, y, z));
                    BlockData upBlock = GetBlock(new BlockCoords(x, y + _lod, z));
                    BlockData downBlock = GetBlock(new BlockCoords(x, y - _lod, z));
                    BlockData southBlock = GetBlock(new BlockCoords(x, y, z + _lod));
                    BlockData northBlock = GetBlock(new BlockCoords(x, y, z - _lod));

                    // if (!ChunkData.IsValidIndex(ChunkData.GetIndex(new BlockCoords(x, y + 1, z))))
                    // 	GD.PrintErr($"Block is of type {upBlock.Type}");
                    //
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

                    IBlockType blockType = new Stone();
                    if (y > -4)
                        blockType = (renderMode & (int)BlockDirection.Up) != 0 ? new Grass() : new Dirt();

                    if (AddFace(new BlockCoords(x, y, z), block, renderMode, blockType))
                    {
                        hasAddedAFace = true;
                    }
                }
            }
        }

        if (!hasAddedAFace)
        {
            return;
        }
        var arrMesh = new ArrayMesh();
        var surfaceArray = _meshRenderer.GetSurfaceArray();

        // No blendshapes, lods, or compression used.
        arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);

        if (arrMesh.GetSurfaceCount() <= 0)
        {
            return;
        }

        StandardMaterial3D atlasMaterial = new()
        {
            AlbedoTexture = GD.Load<Texture2D>("res://resources/images/textures-atlas.png"),
            CullMode = BaseMaterial3D.CullModeEnum.Back, // optional: controls backface culling
            TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
            // Big performance gains when using this instead of PerPixel.
            ShadingMode = BaseMaterial3D.ShadingModeEnum.PerVertex,
            VertexColorUseAsAlbedo = true,
            // DisableReceiveShadows = true,
        };

        arrMesh.SurfaceSetMaterial(0, atlasMaterial);

        _tempMesh = arrMesh;
        _hasRendered = true;
        _lastRerenderTime = Time.GetTicksMsec();
        _isRendering = false;
    }

    // This old system will be removed once mesh generation is finished.
    // It should increase performance by a lot. And allow mesh generation to be done in a separate thread.
    // The tradeoff is that we cannot use godot nodes anymore.
    private bool AddFace(BlockCoords coords, BlockData block, int renderMode, IBlockType blockType)
    {
        if (renderMode == 0)
            return false;

        if (block.Type == BlockType.Air)
            return false;

        var blockPosition = new Vector3(coords.X, coords.Y, coords.Z);

        if ((renderMode & (int)BlockDirection.East) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.East);
            var color = blockType.Color.GetDirectionColor(BlockDirection.East);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.East, texture, color, _lod);
        }
        if ((renderMode & (int)BlockDirection.West) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.West);
            var color = blockType.Color.GetDirectionColor(BlockDirection.West);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.West, texture, color, _lod);
        }
        if ((renderMode & (int)BlockDirection.Up) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.Up);
            var color = blockType.Color.GetDirectionColor(BlockDirection.Up);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.Up, texture, color, _lod);
        }
        if ((renderMode & (int)BlockDirection.Down) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.Down);
            var color = blockType.Color.GetDirectionColor(BlockDirection.Down);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.Down, texture, color, _lod);
        }
        if ((renderMode & (int)BlockDirection.South) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.South);
            var color = blockType.Color.GetDirectionColor(BlockDirection.South);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.South, texture, color, _lod);
        }
        if ((renderMode & (int)BlockDirection.North) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.North);
            var color = blockType.Color.GetDirectionColor(BlockDirection.North);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.North, texture, color, _lod);
        }
        return true;
    }

    private static bool IsBlockOpaque(BlockData blockData)
    {
        return blockData == null || blockData.Type != BlockType.Air;
    }


    public BlockData GetBlock(BlockCoords blockCoords)
    {
        // var worldX = _chunkCoords.X * CHUNK_LENGTH + blockCoords.X;
        // var worldY = _chunkCoords.Y * CHUNK_LENGTH + blockCoords.Y;
        // var worldZ = _chunkCoords.Z * CHUNK_LENGTH + blockCoords.Z;
        //
        // var noiseHeight = Mathf.Sin(worldX * .1) * 10 - Mathf.Sin(worldZ * .1) * 10;
        // var isAir = worldY > noiseHeight;
        //
        // return new BlockData(isAir ? BlockType.Air : BlockType.Stone);

        var blockWorldCoords = GetChunkData().LocalToWorld(blockCoords);

        // If block is in chunk: returns it.
        if (ChunkData.IsValidIndex(ChunkData.GetIndex(blockCoords)))
        {
            return GetChunkData().GetLocalBlock(blockCoords);
        }

        // Else: we have to retrieve it from another chunk in the cache.
        // var blockWorldCoords = GetChunkData().LocalToWorld(blockCoords);
        var chunkCoords = ChunkData.ChunkCoordsFromWorldBlockCoords(blockWorldCoords);
        var chunkFromCache = _chunkCache.GetChunk(chunkCoords);

        if (chunkFromCache != null)
        {
            var relativeBlockCoords = ChunkData.WorldBlockCoordsToLocalBlockCoords(
                    blockWorldCoords,
                    chunkCoords
                );
            return chunkFromCache.GetLocalBlock(
                    relativeBlockCoords
            );
        }

        // If it is not in cache: it needs to be loaded from save file.

        // If it is not in save file: it needs to be generated.
        return null;
    }
}
