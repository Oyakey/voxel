using Godot;
using Voxel.Blocks;

public partial class Dirt : IBlockType
{
    public BlockTexture Texture { get; set; }
    public BlockColor Color { get; set; }

    public Dirt()
    {
        Texture = new BlockTexture(new Vector2I(64, 11));
        Color = new BlockColor(Colors.White);
    }
}
