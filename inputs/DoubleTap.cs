using Godot;

namespace Voxel.Inputs;

public class DoubleTap(string action, float doubleTapTime = 0.3f)
{
    private readonly float _doubleTapTime = doubleTapTime;
    private float _lastPressTime = 0;
    private readonly string _action = action;

    public void UpdateDoubleTap()
    {
        if (IsDoubleTapped)
            IsDoubleTapped = false;

        if (!Input.IsActionJustPressed(_action))
        {
            return;
        }

        if (Time.GetTicksMsec() - _lastPressTime < (_doubleTapTime * 1000f))
        {
            IsDoubleTapped = true;
            _lastPressTime = 0;
            return;
        }

        _lastPressTime = Time.GetTicksMsec();
    }

    public bool IsDoubleTapped = false;
}
