using Godot;
using Voxel.Blocks;

public partial class Stone : IBlockType
{
    public BlockTexture Texture { get; set; }
    public BlockColor Color { get; set; }

    public Stone()
    {
        Texture = new BlockTexture(new Vector2I(40, 40));
        Color = new BlockColor(Colors.White);
    }
}
