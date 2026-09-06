using Godot;
using Voxel.World;

namespace Voxel.World.Biome;

public class BiomeGenerator
{
    public static ChunkData[] GenerateBiomeTerrain(int X, int Z)
    {
        var topBlockType = BlockType.Stone;
        var airBlockType = BlockType.Air;
        ChunkData[] chunks = new ChunkData[Chunk.Y_CHUNKS];

        // Iterate over the whole vertical slice
        for (int yChunkIndex = 0; yChunkIndex < Chunk.Y_CHUNKS; yChunkIndex++)
        {
            var Y = yChunkIndex + Chunk.LOWEST_CHUNK;
            // Init data
            var chunkData = new ChunkData(new ChunkCoords(X, Y, Z));

            // Loop over all the blocks
            for (int y = 0; y < Chunk.CHUNK_LENGTH; y++)
            {
                for (int x = 0; x < Chunk.CHUNK_LENGTH; x++)
                {
                    for (int z = 0; z < Chunk.CHUNK_LENGTH; z++)
                    {

                        var worldX = X * Chunk.CHUNK_LENGTH + x;
                        var worldY = Y * Chunk.CHUNK_LENGTH + y;
                        var worldZ = Z * Chunk.CHUNK_LENGTH + z;

                        var noiseHeight = Mathf.Sin(worldX * .1) * 10 - Mathf.Sin(worldZ * .1) * 10;
                        // var noiseHeight = 12.0;
                        var isAir = worldY > noiseHeight;

                        chunkData.SetBlockAtCoords(
                            new BlockCoords(x, y, z),
                            new BlockData(isAir ? airBlockType : topBlockType)
                        );
                        chunks[yChunkIndex] = chunkData;
                    }
                }
            }
        }
        return chunks;
    }
}
