using Godot;

public partial class Stone : Block
{
    private static readonly CompressedTexture2D texture = ResourceLoader
      .Load<CompressedTexture2D>("res://resources/images/block/stone.png");
}
