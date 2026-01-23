using System;
using System.Globalization;
using System.IO;
using System.Text;
using Godot;

namespace Voxel.World.Save;

public class Saver
{
    public Saver()
    {
        string path = @"testSaves/0_0";
        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(path))
        {
            WriteChunk(sw);
        }

        // Open the file to read from.
        using (StreamReader sr = File.OpenText(path))
        {
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                GD.Print($"{s} - ");
            }
        }
    }

    private void WriteChunk(StreamWriter sw)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    var height = (int)(Mathf.Sin(x * .1) * 10 - Mathf.Sin(z * .1) * 10);

                    byte stone = 1;
                    byte air = 0;
                    byte block = y < height ? stone : air;

                    var id = block.ToString("X", CultureInfo.InvariantCulture);
                    if (id.Length < 2)
                    {
                        id = $"0{id}";
                    }
                    sw.Write($"{id}");
                }
            }
        }
    }
}
