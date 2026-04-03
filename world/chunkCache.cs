using System.Collections.Generic;

namespace Voxel.World;

public class ChunkCache
{
    private readonly Dictionary<ChunkCoords, ChunkData> _cachedChunks = [];

    public bool ContainsChunk(ChunkCoords chunkCoords)
    {
        return _cachedChunks.ContainsKey(chunkCoords);
    }

    public ChunkData GetChunk(ChunkCoords chunkCoords)
    {
        return _cachedChunks[chunkCoords];
    }

    public void AddChunk(ChunkData chunkData)
    {
        if (_cachedChunks.ContainsKey(chunkData.Coords))
        {
            _cachedChunks[chunkData.Coords] = chunkData;
            return;
        }
        _cachedChunks.Add(chunkData.Coords, chunkData);
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
