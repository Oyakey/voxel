using System.Collections.Generic;
using Godot;
using Voxel.World.Biome;

namespace Voxel.World;

public class ChunkGenerator(
        Node3D chunkParent,
        int renderDistance,
        ChunkCache chunkCache,
        RerenderQueue rerenderQueue
)
{
    private readonly Dictionary<ChunkCoords, Chunk> _renderedChunks = [];
    private readonly Node3D _chunkParent = chunkParent;
    private readonly int _renderDistance = renderDistance;
    private readonly ChunkCache _chunkCache = chunkCache;
    private readonly RerenderQueue _rerenderQueue = rerenderQueue;
    public List<Chunk> _chunksToRender = [];

    // public readonly Dictionary<ChunkCoords, ChunkData> ChunkCache = [];

    private void RenderChunk(ChunkCoords chunkCoords)
    {
        if (_renderedChunks.ContainsKey(chunkCoords))
            return;

        HandleAsyncSpawn(new ChunkData(chunkCoords));
    }

    private void GenerateBiomeTerrain(int X, int Z)
    {
        if (_chunkCache.ContainsChunk(new ChunkCoords(X, 0, Z)))
        {
            return;
        }

        GD.Print($"Generating chunk column {X}, {Z}");

        var chunks = BiomeGenerator.GenerateBiomeTerrain(X, Z);

        // Iterate over the whole vertical slice
        for (int index = 0; index < chunks.Length; index++)
        {
            _chunkCache.AddChunk(chunks[index]);
            // Saver.SaveChunk(chunks[index], "testSaves");
        }
    }

    private void HandleAsyncSpawn(ChunkData chunkData)
    {
        var chunk = Chunk.Spawn(chunkData, _chunkCache, _rerenderQueue);
        _renderedChunks.Add(chunkData.Coords, chunk);
        _chunkParent.AddChild(chunk);
    }

    public void RemoveChunk(ChunkCoords chunkCoords)
    {
        _renderedChunks.TryGetValue(chunkCoords, out var chunk);
        if (chunk == null)
        {
            GD.PrintErr($"Tried to remove chunk {chunkCoords} but it wasn't rendered");
            return;
        }
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

    public void RenderChunksAround(ChunkCoords chunkCoords)
    {
        var renderDistanceChunks = CalculateRenderDistanceChunks();
        for (var x = 0; x <= renderDistanceChunks.X; x++)
        {
            for (var z = 0; z <= renderDistanceChunks.Z; z++)
            {
                GenerateBiomeTerrain(
                    chunkCoords.X + x - renderDistanceChunks.X / 2,
                    chunkCoords.Z + z - renderDistanceChunks.Z / 2
                );
                for (var y = 0; y <= renderDistanceChunks.Y; y++)
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
