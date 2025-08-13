using Godot;

public partial class Main : Node3D
{
    public static Node3D Chunk;

    private void _ready()
    {
        var noiseGenerator = new PerlinNoise();

        Chunk = GetNode<Node3D>("Chunk");
        for (var x = 0; x < 16; x++)
        {
            for (var z = 0; z < 16; z++)
            {
                int maxHeight = 16;

                float scale = 8;
                var noise = noiseGenerator.Noise(x / scale, z / scale);

                var height = Mathf.Max(Mathf.Floor(noise * maxHeight), 1);

                for (var y = 0; y < maxHeight; y++)
                    Block.SpawnBlock(Block.GetBlock(new Vector3(x, y - maxHeight, z)));
            }
        }
    }
}
