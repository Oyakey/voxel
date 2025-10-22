using Voxel.Chunk;

namespace Voxel.Tests;

public class ChunkCoords
{
    [Test]
    public void TestCoords()
    {
        Assert.Multiple(() =>
        {
            // Test that the indices are correctly generated.
            Assert.That(
                ChunkData.GetBlockIndex(new BlockCoords(0, 0, 0)),
                Is.EqualTo(0)
            );
            Assert.That(
                ChunkData.GetBlockIndex(new BlockCoords(0, 1, 0)),
                Is.EqualTo(256)
            );
            Assert.That(
                ChunkData.GetBlockIndex(new BlockCoords(1, 0, 0)),
                Is.EqualTo(1)
            );
            Assert.That(
                ChunkData.GetBlockIndex(new BlockCoords(0, 0, 1)),
                Is.EqualTo(16)
            );
            Assert.That(
                ChunkData.GetBlockIndex(new BlockCoords(15, 15, 383)),
                Is.EqualTo(16)
            );

            // Test that the reverse conversion works.
            Assert.That(
                ChunkData.GetBlockCoords(0),
                Is.EqualTo(new BlockCoords(0, 0, 0))
            );
            Assert.That(
                ChunkData.GetBlockCoords(256),
                Is.EqualTo(new BlockCoords(0, 1, 0))
            );
            Assert.That(
                ChunkData.GetBlockCoords(1),
                Is.EqualTo(new BlockCoords(1, 0, 0))
            );
            Assert.That(
                ChunkData.GetBlockCoords(16),
                Is.EqualTo(new BlockCoords(0, 0, 1))
            );
        });

        var arr = new Dictionary<int, int>();
        var index = arr.GetValueOrDefault(123);
        Console.WriteLine($"Dictionary: {index}");

        var arr2 = new int[256];
        Console.WriteLine($"Array: {arr2[123]}");
    }
}
