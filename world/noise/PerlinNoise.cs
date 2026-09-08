using Godot;

namespace Voxel.World.Noise;

public class PerlinNoise
{

    public static float ZoomedPerlinNoise(float x, float y, float zoom = 1, float magnitude = 1)
    {
        return perlinNoise(x / zoom, y / zoom) * magnitude;
    }
    private static float perlinNoise(float x, float y)
    {
        int x0 = Mathf.FloorToInt(x);
        int y0 = Mathf.FloorToInt(y);
        int x1 = x0 + 1;
        int y1 = y0 + 1;

        float sx = x - x0;
        float sy = y - y0;

        var n0 = dotGridGradient(x0, y0, x, y);
        var n1 = dotGridGradient(x1, y0, x, y);
        var ix0 = interpolate(n0, n1, sx);
        var n2 = dotGridGradient(x0, y1, x, y);
        var n3 = dotGridGradient(x1, y1, x, y);
        var ix1 = interpolate(n2, n3, sx);
        return interpolate(ix0, ix1, sy);
    }

    private static float coordToAngleSeeded(Vector2 coord, float seed)
    {
        // Ajouter le seed aux coordonnées
        Vector2 p = coord + new Vector2(seed, seed * 1.7f);

        // Hash
        float n = p.Dot(new Vector2(127.1f, 311.7f));
        n = Mathf.Sin(n) * 43758.5453123f;
        n -= Mathf.Floor(n);

        return n * 6.28318530718f;
    }

    private static float dotGridGradient(int ix, int iy, float x, float y)
    {
        float dx = x - ix;
        float dy = y - iy;

        float angle = coordToAngleSeeded(new Vector2(ix, iy), 0.0f);

        return dx * Mathf.Cos(angle) + dy * Mathf.Sin(angle);
    }

    private static float interpolate(float a0, float a1, float w)
    {
        return a0 + (a1 - a0) * smoothstep(w);
    }
    private static float smoothstep(float w)
    {
        if (w <= 0.0) return 0.0f;
        if (w >= 1.0) return 1.0f;
        return w * w * (3.0f - 2.0f * w);
    }
}
