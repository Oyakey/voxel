using Voxel.Blocks;

public interface IBlockType
{
    BlockTexture Texture { get; set; }
    BlockColor Color { get; set; }
}
