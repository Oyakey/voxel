using Godot;
using Voxel.Chunk;
using Voxel.Inputs;

namespace Voxel;

public partial class Player : CharacterBody3D
{
    private const float _speed = 6;
    private const float _xSensitivity = 0.002f;
    private const float _ySensitivity = 0.002f;
    private readonly float _jumpForce = Mathf.Sqrt(9.8f * 2);
    private const float _jumpGravity = 9.8f * 2;
    private const float _gravity = 9.8f * 3;
    private Node3D _neck;
    private Camera3D _camera;
    private RayCast3D _rayCast;
    private Blocks.Block _hoveredBlock;
    private bool _isFlying = true;
    private readonly DoubleTap _jumpDoubleTap = new("jump", 0.2f);

    private void _ready()
    {
        _neck = GetNode<Node3D>("Neck");
        _camera = GetNode<Camera3D>("Neck/Camera");
        _rayCast = GetNode<RayCast3D>("Neck/Camera/RayCast");
        Main.Player = this;
    }

    private void _process(float delta)
    {
        HandleMovement(delta);
        HandleLoadChunk();
        _jumpDoubleTap.UpdateDoubleTap();
        HandleDoubleJump();
    }

    private void HandleDoubleJump()
    {
        if (!_jumpDoubleTap.IsDoubleTapped)
        {
            return;
        }

        _isFlying = !_isFlying;
    }

    private void HandleLoadChunk()
    {
        var chunkCoords = ChunkGenerator.GetChunkCoordsByPosition(Position);
        Main.PlayerCurrentChunk = chunkCoords;
        ChunkData chunkData = Main.ChunkGenerator.GetChunk(chunkCoords);
        if (chunkData == null)
            return;

        Main.ChunkGenerator.RenderChunksAround(chunkData.Coords);
    }

    private void _physics_process(float delta)
    {
        if (!_isFlying)
            HandleGravity(delta);

        // HandleHoveringBlock();
    }

    private void HandleHoveringBlock()
    {
        _hoveredBlock?.SetBlockHovered(false);

        if (!_rayCast.IsColliding())
            return;

        var hit = _rayCast.GetCollider();

        if (hit is Blocks.Block block)
        {
            _hoveredBlock = block;
            _hoveredBlock.SetBlockHovered(true);
        }
    }

    private void HandleGravity(float delta)
    {
        var newYVelocity = Velocity.Y - (Velocity.Y > 0 ? _jumpGravity : _gravity) * delta;

        Velocity = new Vector3(
            Velocity.X,
            newYVelocity,
            Velocity.Z
        );

        MoveAndSlide();
    }

    private void HandleMovement(float delta)
    {
        var direction = Input.GetVector("left", "right", "forward", "back");
        direction = direction.Normalized();

        var speedDirection = direction * _speed;

        var movementVector = new Vector3(
            speedDirection.X,
            Velocity.Y,
            speedDirection.Y
        );

        if (Input.IsActionJustPressed("jump") && !_isFlying)
        {
            movementVector = Vector3.Up * _jumpForce;
        }

        if (_isFlying)
        {
            movementVector.Y = 0;
            if (Input.IsActionPressed("jump"))
            {
                movementVector.Y = _speed;
            }
            if (Input.IsActionPressed("crouch"))
            {
                movementVector.Y = -_speed;
            }
        }

        var rotationAxis = _neck.Rotation.Normalized();
        var rotationLength = _neck.Rotation.Length();
        Velocity = rotationAxis == Vector3.Zero ? movementVector : movementVector.Rotated(rotationAxis, rotationLength);

        MoveAndSlide();
    }

    private void _unhandled_input(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            Input.SetMouseMode(Input.MouseModeEnum.Captured);
        }
        else if (@event.IsActionPressed("ui_cancel"))
        {
            Input.SetMouseMode(Input.MouseModeEnum.Visible);
        }
        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            if (@event is InputEventMouseMotion ev)
            {
                var yRotation = -ev.Relative.X * _ySensitivity;
                var xRotation = -ev.Relative.Y * _xSensitivity;
                _neck.RotateY(yRotation);
                _camera.Rotation += Vector3.Right * xRotation;
                if (_camera.Rotation.X >= Mathf.Pi / 2)
                {
                    _camera.Rotation = new Vector3(Mathf.Pi / 2, _camera.Rotation.Y, _camera.Rotation.Z);
                }
                _camera.Rotation += Vector3.Right * xRotation;
                // GD.Print(_camera.RotationDegrees);
                if (Mathf.Abs(_camera.RotationDegrees.X + xRotation) < 90)
                {
                    _camera.RotateX(xRotation);
                }
                else
                {
                    _camera.RotationDegrees = new Vector3(
                        _camera.RotationDegrees.X + xRotation >= 0 ? 90 : -90,
                        _camera.RotationDegrees.Y,
                        _camera.RotationDegrees.Z
                    );
                }
            }
        }
    }
}
