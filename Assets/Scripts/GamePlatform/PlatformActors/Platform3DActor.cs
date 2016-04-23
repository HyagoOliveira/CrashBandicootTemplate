using System;
using UnityEngine;

public class Platform3DActor : PlatformActor
{
    public override void UpdateGroundedHorizontalMovement()
    {
        Vector3 direction = GetCameraDirection();

        if (input.IsMoving)
        {
            LookingDirection = direction;
            UpdateMeshRotation();
        }

        moveDirection.x = speed * direction.x;
        moveDirection.z = speed * direction.z;
    }

    protected virtual Vector3 GetCameraDirection()
    {
        Vector3 camForward = Vector3.Scale(
            Camera.main.transform.forward, new Vector3(1f, 0f, 1f)).normalized;
        // Target direction relative to camera
        return input.MovementInput.x * Camera.main.transform.right + input.MovementInput.y * camForward;
    }

    public override void UpdateMeshRotation()
    {
        // Rotate our mesh to face where we are "looking"
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(LookingDirection),
            Time.deltaTime * rotateSpeed);
        animatedMesh.rotation = transform.rotation;
    }


    public override void OnPause()
    {
        throw new NotImplementedException();
    }

    public override void OnRespawn()
    {
        throw new NotImplementedException();
    }

    public override void OnResume()
    {
        throw new NotImplementedException();
    }

    public override void UpdateActor()
    {
    }    

    public override void UpdateGroundedAnimation()
    {
        //throw new NotImplementedException();
    }  
}
