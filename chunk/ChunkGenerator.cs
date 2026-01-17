using System.Collections.Generic;
using Godot;

namespace Voxel.Chunk;

public class ChunkGenerator(Node3D chunkParent, int renderDistance)
{
    private readonly Dictionary<ChunkCoords, ChunkData> _chunks = [];
    private readonly Dictionary<ChunkCoords, Chunk> _renderedChunks = [];
    private readonly Dictionary<ChunkCoords, bool> _renderingChunks = [];
    private readonly Node3D _chunkParent = chunkParent;
    private readonly int _renderDistance = renderDistance;
    public List<Chunk> _chunksToRender = [];

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
        if (_renderedChunks.ContainsKey(chunkCoords) || _renderingChunks.ContainsKey(chunkCoords) || chunkCoords.Y > Chunk.MAX_HEIGHT / Chunk.CHUNK_LENGTH || chunkCoords.Y < Chunk.MIN_HEIGHT / Chunk.CHUNK_LENGTH)
            return;

        _renderingChunks.Add(chunkCoords, true);

        // Generate all neighboring chunks before rendering.
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                for (var z = -1; z <= 1; z++)
                {
                    GenerateChunk(new ChunkCoords(chunkCoords.X + x, chunkCoords.Y + y, chunkCoords.Z + z));
                }
            }
        }

        var chunkData = GetChunk(chunkCoords);

        // new Task(() => HandleAsyncSpawn(chunkData)).Start();
        HandleAsyncSpawn(chunkData);
    }

    private void HandleAsyncSpawn(ChunkData chunkData)
    {
        var chunk = Chunk.Spawn(chunkData);

        _renderedChunks.Add(chunkData.Coords, chunk);
        _renderingChunks.Remove(chunkData.Coords);

        _chunksToRender.Add(chunk);
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
           Mathf.FloorToInt(position.X / Chunk.CHUNK_LENGTH),
           Mathf.FloorToInt(position.Y / Chunk.CHUNK_LENGTH),
           Mathf.FloorToInt(position.Z / Chunk.CHUNK_LENGTH)
       );
    }

    public ChunkData GetChunkByPosition(Vector3 position)
    {
        var chunkCoords = GetChunkCoordsByPosition(position);
        return GetChunk(chunkCoords);
    }

    public void RenderChunksAround(ChunkCoords chunkCoords)
    {
        var renderDistanceChunks = CalculateRenderDistanceChunks();
        for (var x = 0; x <= renderDistanceChunks.X; x++)
        {
            for (var y = 0; y <= renderDistanceChunks.Y; y++)
            {
                for (var z = 0; z <= renderDistanceChunks.Z; z++)
                {
                    RenderChunk(new ChunkCoords(
                        chunkCoords.X + x - renderDistanceChunks.X / 2,
                        y + Chunk.LOWEST_CHUNK,
                        chunkCoords.Z + z - renderDistanceChunks.Z / 2
                    ));
                }
            }
        }
    }

    public Vector3I CalculateRenderDistanceChunks()
    {
        var distanceXZ = _renderDistance * 2 + 1;
        return new Vector3I(
            distanceXZ,
            Chunk.Y_CHUNKS,
            distanceXZ);
    }
}
