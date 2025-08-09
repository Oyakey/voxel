using Godot;

public partial class Player : CharacterBody3D
{
    private const float _speed = 20;

    private void _process(float delta)
    {
        var direction = new Vector3(
            Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
            0,
            Input.GetActionStrength("move_forward") - Input.GetActionStrength("move_back")
        );
        direction = direction.Normalized();
        MoveAndCollide(direction * delta * _speed);
    }
}
