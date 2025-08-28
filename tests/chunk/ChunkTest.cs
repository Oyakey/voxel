using Voxel.Chunk;

namespace Voxel.Tests.Chunk;

class ChunkTest : ITest
{
    public void Run()
    {
        var chunk = new ChunkData(new ChunkCoords(0, 0));
    }
}
