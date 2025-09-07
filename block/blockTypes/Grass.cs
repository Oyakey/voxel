using Godot;
using Voxel.Blocks;

public partial class Grass : IBlockType
{
    public BlockTexture Texture { get; set; }
    public BlockColor Color { get; set; }

    public Grass()
    {
        Texture = new BlockTexture(new Vector2I(25, 36), new Vector2I(22, 36), new Vector2I(64, 11));
        Color = new BlockColor(Colors.YellowGreen, Colors.White, Colors.White);
    }
}
