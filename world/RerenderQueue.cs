using System.Collections.Generic;
using Godot;

namespace Voxel.World;

public class RerenderQueue
{
    private readonly Dictionary<ChunkCoords, ulong> _chunksToRender = [];

    public bool ShouldRender(ChunkCoords chunkCoords)
    {
        return _chunksToRender.ContainsKey(chunkCoords);
    }

    public ulong? GetRerenderTime(ChunkCoords chunkCoords)
    {
        try
        {
            return _chunksToRender[chunkCoords];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public void AddChunk(ChunkCoords chunkCoords)
    {
        if (_chunksToRender.ContainsKey(chunkCoords))
        {
            _chunksToRender[chunkCoords] = Time.GetTicksMsec();
            return;
        }
        _chunksToRender.Add(chunkCoords, Time.GetTicksMsec());
    }

    public void RemoveChunk(ChunkCoords chunkCoords)
    {
        _chunksToRender.Remove(chunkCoords);
    }

    public void Clear()
    {
        _chunksToRender.Clear();
    }
}
