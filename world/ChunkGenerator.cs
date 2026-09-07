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

    private void RenderChunk(ChunkCoords chunkCoords, ushort lod)
    {
        if (_renderedChunks.ContainsKey(chunkCoords))
            return;

        HandleAsyncSpawn(new ChunkData(chunkCoords), lod);
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

    private void HandleAsyncSpawn(ChunkData chunkData, ushort lod)
    {
        var chunk = Chunk.Spawn(chunkData, _chunkCache, _rerenderQueue, lod);
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

    // TODO: Make this work with the LOD
    // Chunks should be rerendered when there lod changes (when the player moves)
    // Add a priotity based on distance to the player
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
                    var distance = Mathf.Max(
                        Mathf.Abs(x - renderDistanceChunks.X / 2),
                        Mathf.Abs(z - renderDistanceChunks.Z / 2)
                    );
                    ushort lod = (ushort)(
                        distance < 6 ? 1 :
                        distance < 8 ? 2 :
                        distance < 12 ? 4 :
                        distance < 16 ? 8 :
                        16);
                    RenderChunk(new ChunkCoords(
                        chunkCoords.X + x - renderDistanceChunks.X / 2,
                        y + Chunk.LOWEST_CHUNK,
                        chunkCoords.Z + z - renderDistanceChunks.Z / 2
                    ), lod);
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
