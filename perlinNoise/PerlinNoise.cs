using Godot;
using System.Collections.Generic;

public class PerlinNoise
{
    private RandomNumberGenerator rng = new();

    public PerlinNoise()
    {
        rng.Seed = 123456789;
    }

    private Dictionary<(int x, int y), SerializableVector2> _gradients = new();

    public float Noise(float x, float y)
    {
        int x0 = Mathf.FloorToInt(x);
        int x1 = x0 + 1;
        int y0 = Mathf.FloorToInt(y);
        int y1 = y0 + 1;

        float sx = x - x0;
        float sy = y - y0;

        float n0, n1, ix0, ix1, value;
        n0 = DotGridGradient(x0, y0, x, y);
        n1 = DotGridGradient(x1, y0, x, y);
        ix0 = Interpolate(n0, n1, sx);
        n0 = DotGridGradient(x0, y1, x, y);
        n1 = DotGridGradient(x1, y1, x, y);
        ix1 = Interpolate(n0, n1, sx);
        value = Interpolate(ix0, ix1, sy);

        return value;
    }

    private static float Smoothstep(float w)
    {
        if (w <= 0.0) return 0.0f;
        if (w >= 1.0) return 1.0f;
        return w * w * (3.0f - 2.0f * w);
    }

    private static float Interpolate(float a0, float a1, float w)
    {
        return a0 + (a1 - a0) * Smoothstep(w);
    }

    private float DotGridGradient(int ix, int iy, float x, float y)
    {
        float dx = x - ix;
        float dy = y - iy;

        var gradient = GetGradient(ix, iy);

        return dx * gradient.X + dy * gradient.Y;
    }

    private SerializableVector2 GetGradient(int ix, int iy)
    {
        _gradients.TryGetValue((iy, ix), out var gradient);
        if (gradient != null)
        {
            return gradient;
        }

        var newGradient = new SerializableVector2(rng.Randf() * 2 - 1, rng.Randf() * 2 - 1);

        var newGradientLength = Mathf.Sqrt(Mathf.Pow(newGradient.X, 2) + Mathf.Pow(newGradient.Y, 2));

        var normalizedNewGradient = new SerializableVector2(newGradient.X / newGradientLength, newGradient.Y / newGradientLength);

        _gradients.Add((iy, ix), normalizedNewGradient);

        return normalizedNewGradient;
    }
}
