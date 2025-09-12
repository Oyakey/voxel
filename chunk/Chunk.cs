using System;
using Godot;
using Voxel.Blocks;

namespace Voxel.Chunk;

public partial class Chunk : MeshInstance3D
{
    public const int SIDE_LENGTH = 16;
    public const int MIN_HEIGHT = -8;
    public const int MAX_HEIGHT = 8;

    private long _worldSeed = 123456789;
    private long _chunkSeed;

    private readonly MeshRenderer _meshRenderer = new();
    private ChunkData _chunkData;

    private static readonly PackedScene chunkPrefab = ResourceLoader
      .Load<PackedScene>("res://chunk/chunk.tscn");

    private MeshInstance3D _opaqueMesh;

    public static Chunk Spawn(ChunkData chunkData)
    {
        var chunk = chunkPrefab.Instantiate<Chunk>();
        chunk._chunkData = chunkData;
        var x = chunkData.Coords.X;
        var y = chunkData.Coords.Y;
        chunk.Position = new Vector3(x * SIDE_LENGTH, 0, y * SIDE_LENGTH);
        chunk.renderBlocks();
        return chunk;
    }

    private void _process(float _)
    {
        var chunkCoords = Main.PlayerCurrentChunk;

        var distanceXFromPlayer = Math.Abs(_chunkData.Coords.X - chunkCoords.X);
        var distanceYFromPlayer = Math.Abs(_chunkData.Coords.Y - chunkCoords.Y);

        if (distanceXFromPlayer > Main.RenderDistance || distanceYFromPlayer > Main.RenderDistance)
        {
            Main.ChunkGenerator.RemoveChunk(_chunkData.Coords);
        }
    }

    private void renderBlocks()
    {
        // We need to check what blocks should be rendered
        // They are :
        // 1. Blocks that are in the chunk (in _blocks)
        // 2. Blocks that are NOT surrounded by blocks on all sides
        // 3. Theses blocks should have only their visible faces rendered
        // We also need to make this operation asynchronous so that it won't freeze the game

        Godot.Collections.Array surfaceArray = [];
        surfaceArray.Resize((int)Mesh.ArrayType.Max);

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

        surfaceArray[(int)Mesh.ArrayType.Vertex] = _meshRenderer.Verts.ToArray();
        // surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Normal] = _meshRenderer.Normals.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index] = _meshRenderer.Indices.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Color] = _meshRenderer.Colors.ToArray();

        var arrMesh = new ArrayMesh();
        // No blendshapes, lods, or compression used.
        arrMesh?.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
        Mesh = arrMesh;
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

}
