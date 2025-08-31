using System.Collections.Generic;
using System.Threading;
using Godot;

namespace Voxel.Chunk;

public class ChunkGenerator(Node3D chunkParent)
{
    private readonly Dictionary<ChunkCoords, ChunkData> _chunks = [];
    private readonly Dictionary<ChunkCoords, Chunk> _renderedChunks = [];
    private readonly Node3D _chunkParent = chunkParent;
    private object _lock = new object();

    public ChunkData GetChunk(ChunkCoords chunkCoords)
    {
        _chunks.TryGetValue(chunkCoords, out var chunk);
        return chunk;
    }

    public void GenerateChunk(ChunkCoords chunkCoords)
    {
        if (_chunks.ContainsKey(chunkCoords))
            return;
        var chunk = new ChunkData(chunkCoords);
        _chunks.Add(chunkCoords, chunk);
    }

    public void RenderChunk(ChunkCoords chunkCoords)
    {
        if (_renderedChunks.ContainsKey(chunkCoords))
            return;

        var generationStartTime = Time.GetTicksUsec();
        // Generate all neighboring chunks before rendering.
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                GenerateChunk(new ChunkCoords(chunkCoords.X + x, chunkCoords.Y + y));
            }
        }
        var spawnStartTime = Time.GetTicksUsec();

        var chunkData = GetChunk(chunkCoords);

        var chunk = Chunk.Spawn(chunkData);

        _renderedChunks.Add(chunkCoords, chunk);

        _chunkParent.CallDeferred("add_child", chunk);

        var spawnEndTime = Time.GetTicksUsec();
        var generationDuration = spawnStartTime - generationStartTime;
        var spawnDuration = spawnEndTime - spawnStartTime;
        GD.Print($"Generation duration: {generationDuration} spawn duration: {spawnDuration}");
    }

    public void RemoveChunk(ChunkCoords chunkCoords)
    {
        _renderedChunks.TryGetValue(chunkCoords, out var chunk);
        if (chunk == null)
            return;
        _chunkParent.RemoveChild(chunk);
        _renderedChunks.Remove(chunkCoords);
    }

    public static ChunkCoords GetChunkCoordsByPosition(Vector3 position)
    {
        return new ChunkCoords(
           Mathf.FloorToInt(position.X / Chunk.SIDE_LENGTH),
           Mathf.FloorToInt(position.Z / Chunk.SIDE_LENGTH)
       );
    }

    public ChunkData GetChunkByPosition(Vector3 position)
    {
        var chunkCoords = GetChunkCoordsByPosition(position);
        return GetChunk(chunkCoords);
    }

    public void RenderChunksAround(ChunkCoords chunkCoords)
    {
        Thread thread = new Thread(() =>
        {
            var startTime = Time.GetTicksMsec();
            // GD.Print($"Started on {startTime}");

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    RenderChunk(new ChunkCoords(chunkCoords.X + x, chunkCoords.Y + y));
                }
            }

            var endTime = Time.GetTicksMsec();
            // GD.Print($"Ended on {endTime}");
            if (endTime - startTime > 1)
                GD.Print($"Total time elapsed: {endTime - startTime}\n==========");
        });
        thread.Start();
    }
}
