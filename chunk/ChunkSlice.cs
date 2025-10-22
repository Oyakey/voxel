using System.Threading;
using Godot;
using Voxel.Blocks;

namespace Voxel.Chunk;

public partial class ChunkSlice : MeshInstance3D
{
    private CollisionShape3D _collisionShape;
    private Mesh _tempMesh;
    private bool _isRendering = false;
    private byte _layer;
    private StandardMaterial3D _atlasMaterial;
    private readonly MeshRenderer _meshRenderer = new();
    private readonly ChunkData _chunkData;
    private readonly int _sliceIndex;

    public ChunkSlice(int sliceIndex, ChunkData chunkData)
    {
        _chunkData = chunkData;
        _sliceIndex = sliceIndex;

        _atlasMaterial = new()
        {
            AlbedoTexture = GD.Load<Texture2D>("res://resources/images/textures-atlas.png"),
            CullMode = BaseMaterial3D.CullModeEnum.Back,
            TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.PerVertex, // Big performance gains when using this instead of PerPixel.
            VertexColorUseAsAlbedo = true,
        };
    }

    private void RenderOnThread()
    {
        new Thread(RenderSlice).Start();
    }

    private void RenderSlice()
    {
        // We need to check what blocks should be rendered
        // They are :
        // 1. Blocks that are in the chunk (in _blocks)
        // 2. Blocks that are NOT surrounded by blocks on all sides
        // 3. Theses blocks should have only their visible faces rendered
        // We also need to make this operation asynchronous so that it won't freeze the game
        _isRendering = true;

        var sliceMinHeight = Chunk.MIN_HEIGHT + _sliceIndex * Chunk.SIDE_LENGTH;
        var sliceMaxHeight = sliceMinHeight + Chunk.SIDE_LENGTH;

        for (var x = 0; x < Chunk.SIDE_LENGTH; x++)
        {
            for (var z = 0; z < Chunk.SIDE_LENGTH; z++)
            {
                for (var y = sliceMinHeight; y < sliceMaxHeight; y++)
                {
                    RenderBlock(x, y, z);
                }
            }
        }

        var surfaceArray = _meshRenderer.GetSurfaceArray();
        var arrMesh = new ArrayMesh();
        // No blendshapes, lods, or compression used.
        arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
        arrMesh.SurfaceSetMaterial(0, _atlasMaterial);

        _tempMesh = arrMesh;
        _isRendering = false;
    }

    private void RenderBlock(int x, int y, int z)
    {
        int renderMode = 0;
        var block = _chunkData.GetBlock(new BlockCoords(x, y, z));
        if (block == null)
            return;

        BlockData eastBlock = _chunkData.GetBlock(new BlockCoords(x + 1, y, z));
        BlockData westBlock = _chunkData.GetBlock(new BlockCoords(x - 1, y, z));
        BlockData upBlock = _chunkData.GetBlock(new BlockCoords(x, y + 1, z));
        BlockData downBlock = _chunkData.GetBlock(new BlockCoords(x, y - 1, z));
        BlockData southBlock = _chunkData.GetBlock(new BlockCoords(x, y, z + 1));
        BlockData northBlock = _chunkData.GetBlock(new BlockCoords(x, y, z - 1));

        if (!Chunk.IsBlockOpaque(eastBlock))
            renderMode += (int)BlockDirection.East;
        if (!Chunk.IsBlockOpaque(westBlock))
            renderMode += (int)BlockDirection.West;
        if (!Chunk.IsBlockOpaque(upBlock))
            renderMode += (int)BlockDirection.Up;
        if (!Chunk.IsBlockOpaque(downBlock))
            renderMode += (int)BlockDirection.Down;
        if (!Chunk.IsBlockOpaque(southBlock))
            renderMode += (int)BlockDirection.South;
        if (!Chunk.IsBlockOpaque(northBlock))
            renderMode += (int)BlockDirection.North;

        IBlockType blockType = new Stone();
        if (y > -4)
            blockType = (renderMode & (int)BlockDirection.Up) != 0 ? new Grass() : new Dirt();

        AddFace(new BlockCoords(x, y, z), block, renderMode, blockType);
    }

    private void AddFace(BlockCoords coords, BlockData block, int renderMode, IBlockType blockType)
    {
        if (renderMode == 0)
            return;

        if (block.Type == BlockType.Air)
            return;

        var blockPosition = new Vector3(coords.X, coords.Y, coords.Z);

        if ((renderMode & (int)BlockDirection.East) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.East);
            var color = blockType.Color.GetDirectionColor(BlockDirection.East);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.East, texture, color);
        }
        if ((renderMode & (int)BlockDirection.West) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.West);
            var color = blockType.Color.GetDirectionColor(BlockDirection.West);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.West, texture, color);
        }
        if ((renderMode & (int)BlockDirection.Up) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.Up);
            var color = blockType.Color.GetDirectionColor(BlockDirection.Up);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.Up, texture, color);
        }
        if ((renderMode & (int)BlockDirection.Down) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.Down);
            var color = blockType.Color.GetDirectionColor(BlockDirection.Down);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.Down, texture, color);
        }
        if ((renderMode & (int)BlockDirection.South) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.South);
            var color = blockType.Color.GetDirectionColor(BlockDirection.South);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.South, texture, color);
        }
        if ((renderMode & (int)BlockDirection.North) != 0)
        {
            var texture = blockType.Texture.GetDirectionTexture(BlockDirection.North);
            var color = blockType.Color.GetDirectionColor(BlockDirection.North);
            _meshRenderer.GenerateQuad(blockPosition, BlockDirection.North, texture, color);
        }
    }
}
