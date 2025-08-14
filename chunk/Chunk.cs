using Godot;
using System.Collections.Generic;

public partial class Chunk : Node3D
{
    private Node3D _chunkNode;
    private Dictionary<(int x, int y, int z), BlockData> _blocks;

    private void _ready()
    {
        _chunkNode = GetNode<Node3D>("Chunk");
        return;

        generateBlocks();
        renderBlocks();

        // Logic for destrying and recreating the chunk will be done later.
    }

    private void generateBlocks()
    {
        var noiseGenerator = new PerlinNoise();

        for (var x = 0; x < 16; x++)
        {
            for (var z = 0; z < 16; z++)
            {
                int maxHeight = 256;

                float scale = 8;
                var noise = noiseGenerator.Noise(x / scale, z / scale);

                var height = Mathf.Max(Mathf.Floor(noise * maxHeight), 1);

                for (var y = -maxHeight; y < 0; y++)
                    _blocks.Add((x, y, z), Block.GetBlock(new Vector3(x, y, z)));
            }
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
        for (var x = 0; x < 16; x++)
        {
            for (var z = 0; z < 16; z++)
            {
                for (var y = 0; y < 256; y++)
                {
                    _blocks.TryGetValue((x, y, z), out var block);
                    int renderMode = 0;
                    if (block == null)
                    {
                        return;
                    }
                    Block.RenderBlock(block, renderMode);
                }
            }
        }
    }
}
