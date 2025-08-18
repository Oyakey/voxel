using Godot;

public partial class Dirt : Block
{
    private static readonly CompressedTexture2D texture = ResourceLoader
      .Load<CompressedTexture2D>("res://resources/images/block/dirt.png");
}
