using Godot;

namespace Voxel.BlockFace;

public partial class Face : MeshInstance3D
{
    private void _process(float delta)
    {
        HandleBackFaceCulling();
    }

    private void HandleBackFaceCulling()
    {
        // Get main camera to check if the face is facing the camera
        if (IsFacingCamera() && !Visible)
        {
            Visible = true;
            return;
        }
        if (!IsFacingCamera() && Visible)
        {
            Visible = false;
        }
    }

    private bool IsFacingCamera()
    {
        var camera = GetViewport().GetCamera3D();
        if (camera == null)
            return false;
        var cameraTransform = camera.GlobalTransform;
        var faceTransform = GlobalTransform;
        var faceDirection = faceTransform.Basis.Z;
        var cameraDirection = cameraTransform.Basis.Z;
        return Rotation.Dot(camera.Rotation) > 0;
        // return faceDirection.Dot(cameraDirection) < 0;
    }
}
