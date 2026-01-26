using System;
using System.IO;
using Godot;
using Voxel.Chunk;

namespace Voxel.World.Save;

public static class Saver
{
    private const byte VERSION = 3;
    private const int HEADER_SIZE = 9;
    private const int BLOCKS_COUNT = 4096;

    public static void SaveChunk(ChunkData chunk, string saveDir)
    {
        Directory.CreateDirectory(saveDir);
        string path = GetChunkPath(chunk.Coords, saveDir);
        using var writer = new BinaryWriter(File.Create(path));
        // Write header
        writer.Write(VERSION);
        writer.Write(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        // Write blocks
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    writer.Write((byte)chunk.GetBlock(new BlockCoords(x, y, z)).Type);
                }
            }
        }
    }

    public static BlockData[] LoadChunk(ChunkCoords coords, string saveDir)
    {
        string path = GetChunkPath(coords, saveDir);
        if (!File.Exists(path)) return null;

        using var reader = new BinaryReader(File.OpenRead(path));
        byte version = reader.ReadByte();
        // Skip timestamp (8 bytes)
        reader.ReadInt64();

        var blocks = new BlockData[BLOCKS_COUNT];
        for (int i = 0; i < BLOCKS_COUNT; i++)
            blocks[i] = new BlockData((BlockType)reader.ReadByte());
        return blocks;
    }

    private static string GetChunkPath(ChunkCoords coords, string saveDir)
        => $"{saveDir}/chunk_{coords.X}_{coords.Y}_{coords.Z}.bin";
}
