using Godot;
using System.Collections.Generic;

public class PerlinNoise
{
    private Dictionary<(int x, int y), Vector2> _gradients = new Dictionary<(int x, int y), Vector2>();

    public float Noise(float x, float y)
    {
        int x0 = (int)Mathf.Floor(x);
        int x1 = x0 + 1;
        int y0 = (int)Mathf.Floor(y);
        int y1 = y0 + 1;

        float dx = x - x0;
        float dy = y - y0;

        var a = DotGradient(x0, y0, x, y);
        var b = DotGradient(x1, y0, x, y);
        var c = DotGradient(x0, y1, x, y);
        var d = DotGradient(x1, y1, x, y);

        var ab = Interpolate(a, b, dx);
        var cd = Interpolate(c, d, dx);

        return Interpolate(ab, cd, dy);
    }

    private static float Smoothstep(float w)
    {
        if (w <= 0)
            return 0;
        if (w >= 1)
            return 1;
        return w * w * (3 - 2 * w);
    }

    private static float Interpolate(float a, float b, float w)
    {
        return a + (b - a) * w;
    }

    private float DotGradient(int ix, int iy, float x, float y)
    {
        float dx = x - ix;
        float dy = y - iy;

        return GetGradient(ix, iy).Dot(new Vector2(dx, dy));
    }

    private Vector2 GetGradient(int ix, int iy)
    {
        _gradients.TryGetValue((iy, ix), out var gradient);
        if (gradient.Length() != 0)
        {
            return gradient;
        }

        var rng = new RandomNumberGenerator();
        rng.Seed = 123456789;

        var newGradient = new Vector2(rng.Randf(), rng.Randf());
        var normalizedNewGradient = newGradient.Normalized();

        _gradients.Add((iy, ix), normalizedNewGradient);

        return normalizedNewGradient;
    }

    public Vector2[] TestGradients = [
        new Vector2(0.6062291552173583f, -0.7952900171411986f),
        new Vector2(-0.7763827523981961f, 0.6302617089579545f),
        new Vector2(-0.8953522206876869f, -0.44535873283189104f),
        new Vector2(-0.3854188417633184f, -0.9227417387404897f),
        new Vector2(0.7882282548114128f, -0.6153829850726737f),
        new Vector2(-0.4075310917638578f, -0.913191332222201f),
        new Vector2(0.30003134297023853f, -0.9539293439429751f),
        new Vector2(0.3648653589238747f, 0.9310602933523435f),
        new Vector2(-0.25872097344851286f, -0.9659520991735842f),
        new Vector2(0.8412833006368445f, 0.5405944950418721f),
        new Vector2(-0.43655230984822047f, -0.8996788764699232f),
        new Vector2(0.677706947251552f, 0.735332097522597f),
        new Vector2(-0.9644054886181435f, 0.26442778508167425f),
        new Vector2(-0.6789976132804156f, 0.7341404778102751f),
        new Vector2(0.9034447589729978f, -0.42870452235102663f),
        new Vector2(-0.4933606812364599f, -0.8698248319115155f),
        new Vector2(-0.5863087942723123f, 0.8100876481955194f),
        new Vector2(-0.4823591072479644f, -0.8759735678974262f),
        new Vector2(0.9877174054973461f, 0.1562508460060011f),
        new Vector2(-0.9882745831547445f, -0.15268709274302147f),
        new Vector2(0.10107083264258533f, 0.994879232263361f),
        new Vector2(-0.5925338274900248f, 0.8055455687172647f),
        new Vector2(0.6237248647608054f, 0.7816439682356124f),
        new Vector2(-0.9814924093959917f, 0.19150104516177172f),
        new Vector2(0.43240716310778626f, 0.901678460036102f),
        new Vector2(0.6563757204982398f, 0.754434167797573f),
        new Vector2(0.3159977686833441f, -0.9487599328529571f),
        new Vector2(0.34663020357856017f, -0.9380018667183374f),
        new Vector2(-0.8494851311728839f, 0.5276125585277402f),
        new Vector2(0.9772192338665027f, -0.2122323466424603f),
        new Vector2(0.5785155855637508f, -0.8156713291883136f),
        new Vector2(-0.2320132625113007f, -0.9727126225246912f),
        new Vector2(-0.9628896474479253f, -0.2698953998081664f),
        new Vector2(-0.935324660028231f, -0.3537905882624271f),
        new Vector2(0.9669165913526745f, -0.2550927387577415f),
        new Vector2(0.5243358346936758f, -0.8515115574412869f),
        new Vector2(-0.9423005971130769f, -0.3347679564718504f),
        new Vector2(0.5869898315545962f, 0.8095943043596012f),
        new Vector2(-0.9995017496172495f, 0.03156346799793592f),
        new Vector2(-0.6080192226099561f, 0.7939223040932812f),
        new Vector2(0.9630440295715506f, -0.26934401256866747f),
        new Vector2(-0.18186261242918336f, -0.9833239497746573f),
        new Vector2(0.11980296627099142f, 0.9927976879871708f),
        new Vector2(-0.6198666745514069f, 0.784707146507906f),
        new Vector2(0.6318602076212623f, 0.7750823685420896f),
        new Vector2(0.6610877440916185f, 0.7503085995854337f),
        new Vector2(-0.13745592195068213f, -0.9905078846332763f),
        new Vector2(0.7056354278523584f, -0.7085750792679764f),
        new Vector2(-0.8504935836149753f, 0.5259854220696207f),
        new Vector2(0.5842593724301814f, 0.8115669939860114f),
        new Vector2(-0.08693231075119398f, 0.9962142206109376f),
        new Vector2(0.594113172748637f, -0.8043814629680051f),
        new Vector2(-0.7295967431478101f, 0.683877615065816f),
        new Vector2(-0.43303577626446305f, 0.9013767339326182f),
        new Vector2(-0.3230827397775549f, -0.9463707218938193f),
        new Vector2(-0.970861386455249f, 0.23964174988968778f),
        new Vector2(0.11013849685616427f, 0.9939162497465591f),
        new Vector2(-0.8642320808200437f, -0.5030933417184705f),
        new Vector2(-0.6247236515860799f, -0.7808459253584886f),
        new Vector2(-0.964484796361011f, 0.26413836826265696f),
        new Vector2(-0.24689960084118073f, 0.9690410657471982f),
        new Vector2(-0.6840164071473935f, -0.7294666234675656f),
        new Vector2(-0.3350239104750827f, -0.942209626044005f),
        new Vector2(0.9155480322079258f, 0.4022086532139692f)
    ];
}
