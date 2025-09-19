using Godot;
using Voxel.Chunk;
using Voxel.Inputs;

namespace Voxel;

public partial class Player : CharacterBody3D
{
    private const float _speed = 4;
    private const float _flySpeed = 10;
    private const float _xSensitivity = 0.002f;
    private const float _ySensitivity = 0.002f;
    private readonly float _jumpForce = Mathf.Sqrt(9.8f * 2);
    private const float _jumpGravity = 9.8f * 2;
    private const float _gravity = 9.8f * 3;
    private Node3D _neck;
    private Camera3D _camera;
    private RayCast3D _rayCast;
    private Vector3I _hoveredBlock;
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
        HandleBreakingBlock();
    }

    private const float _attackCooldown = 0.5f;
    private float _lastAttackTime = 0;
    private void HandleBreakingBlock()
    {
        if (
            !Input.IsActionPressed("attack") ||
            !_rayCast.IsColliding() ||
            Time.GetTicksMsec() - _lastAttackTime < (_attackCooldown * 1000)
        )
        {
            return;
        }
        _lastAttackTime = Time.GetTicksMsec();
        if (_rayCast.GetCollider() is StaticBody3D body)
        {
            if (body.GetParent() is Chunk.Chunk chunk)
            {
                var hoveredBlock = HitToBlockCoords(_rayCast.GetCollisionPoint(), _rayCast.GetCollisionNormal());
                chunk.BreakBlock(new BlockCoords(hoveredBlock.X, hoveredBlock.Y, hoveredBlock.Z));
            }
        }
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

        HandleHoveringBlock();
    }

    private static void HandleHoveringBlock()
    {
        // if (!_rayCast.IsColliding())
        //     return;
        //
        // _hoveredBlock = HitToBlockCoords(_rayCast.GetCollisionPoint(), _rayCast.GetCollisionNormal());
    }

    public static Vector3I HitToBlockCoords(Vector3 hit, Vector3 normal)
    {
        // There are three things to consider for the hit to block coords conversion:
        // 1. The raycast can sometime hit inside a block, but the point SHOULD be near the hovered face.
        // This is why we need to round the hit position for the axis of the normal.
        //
        // 2. A face can be at a different point of the grid than the block it is from. 
        // A hit from above at (0, 1, 0) should resolve to the block at (0, 0, 0), 
        // since it is the top face of this block that is at (0, 1, 0). It will happen everytime
        // the hit face normal is > 0 (for X, Y and Z axis).
        //
        // 3. This method can't be used to resolve coordinates for a "block" smaller than 1 unit.
        // Like doors in minecraft for example which are like .2 unit thick.
        return new Vector3I(
            normal.X == 0 ? Mathf.FloorToInt(hit.X) : Mathf.RoundToInt(normal.X < 0 ? hit.X : hit.X - 1),
            normal.Y == 0 ? Mathf.FloorToInt(hit.Y) : Mathf.RoundToInt(normal.Y < 0 ? hit.Y : hit.Y - 1),
            normal.Z == 0 ? Mathf.FloorToInt(hit.Z) : Mathf.RoundToInt(normal.Z < 0 ? hit.Z : hit.Z - 1)
        );
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

        var speedDirection = direction * (_isFlying ? _flySpeed : _speed);

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
