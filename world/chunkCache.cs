using System.Collections.Generic;

namespace Voxel.World;

public class ChunkCache(RerenderQueue rerenderQueue)
{
    private readonly Dictionary<ChunkCoords, ChunkData> _cachedChunks = [];
    private readonly RerenderQueue _rerenderQueue = rerenderQueue;

    public bool ContainsChunk(ChunkCoords chunkCoords)
    {
        return _cachedChunks.ContainsKey(chunkCoords);
    }

    public ChunkData GetChunk(ChunkCoords chunkCoords)
    {
        try
        {
            return _cachedChunks[chunkCoords];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public void AddChunk(ChunkData chunkData)
    {
        if (_cachedChunks.ContainsKey(chunkData.Coords))
        {
            _cachedChunks[chunkData.Coords] = chunkData;
            return;
        }
        _cachedChunks.Add(chunkData.Coords, chunkData);

        // Add chunks around the chunk to the rerender queue + the chunk itself
        for (int x = 1; x <= -1; x++)
        {
            for (int y = 1; y <= -1; y++)
            {
                for (int z = 1; z <= -1; z++)
                {
                    var coords = new ChunkCoords(
                        chunkData.Coords.X + x,
                        chunkData.Coords.Y + y,
                        chunkData.Coords.Z + z
                    );
                    if (coords.Y < (Chunk.MIN_HEIGHT / Chunk.CHUNK_LENGTH) ||
                        coords.Y > (Chunk.MAX_HEIGHT / Chunk.CHUNK_LENGTH))
                    {
                        continue;
                    }

                    _rerenderQueue.AddChunk(coords);
                }
            }
        }

    }

    public void RemoveChunk(ChunkCoords chunkCoords)
    {
        _cachedChunks.Remove(chunkCoords);
    }

    public void ClearCache()
    {
        _cachedChunks.Clear();
    }
}
