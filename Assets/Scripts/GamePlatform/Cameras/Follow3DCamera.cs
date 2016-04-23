using System;
using UnityEngine;

/// <summary>
/// Follow 3D Camera.
/// This script allows a free camera rotation around a target.
/// The GO hierarchy must fallows: Camera3DGO -> MainCamera.
/// Camera3DGO is a gameobject with a transform position where you want the camera's focus.
/// (like in traget's center)
/// Put this script inside Camera3DGO.
/// </summary>
public class Follow3DCamera : FollowCamera
{
    public float rotationSpeed = 1.5f;
    public float rotationSmoothing = 0.1f;
    public float maxHorizontalAxisRotation = 75f;       // The maximum value of the x axis rotation of the pivot.
    public float minHorizontalAxisRotation = 45f;
    public bool freezeWhenJump = true;

    protected Vector3 resetTargetForward;
    protected Vector2 smoothVelocity = Vector2.zero;
    protected Vector2 smooth = Vector2.zero;
    protected bool resetCamera = false;    

    private float lookAngle;                            // The rig's y axis rotation.
    private float tiltAngle;                            // The pivot's x axis rotation.

    protected override void UpdateCamera()
    {
        if (resetCamera)
        {
            UpdateResetCamera();
        }
        else
        {
            UpdateCameraRotation();
        }
    }

    protected override void FollowTarget()
    {
        // Move the rig towards target position.
        transform.position = Vector3.Lerp(transform.position, 
            target.transform.position + centerOffset, Time.deltaTime * dampingSpeed);
    }        

    public override void ResetCameraPosition()
    {
        resetTargetForward = target.transform.forward;
        resetCamera = true;
    }

    protected override void UpdateResetCamera()
    {
        Quaternion newrotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(resetTargetForward),
            Time.deltaTime * rotationSpeed * 10f);
        transform.rotation = newrotation;
        transform.RotateAround(target.transform.position, Vector3.up, newrotation.x);
        if (Quaternion.Angle(newrotation, Quaternion.LookRotation(resetTargetForward)) < 1.5f)
        {
            lookAngle = transform.eulerAngles.y;
            tiltAngle = transform.localRotation.x;
            resetCamera = false;
        }
    }

    protected virtual void UpdateCameraRotation()
    {
        float speed = Time.deltaTime * rotationSmoothing;

        // smooth the user input
        smooth.x = Mathf.SmoothDamp(smooth.x, lookInput.x, ref smoothVelocity.x, speed);
        smooth.y = Mathf.SmoothDamp(smooth.y, lookInput.y, ref smoothVelocity.y, speed);

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        lookAngle += smooth.x * rotationSpeed;

        tiltAngle -= smooth.y * rotationSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, -minHorizontalAxisRotation, maxHorizontalAxisRotation);

        transform.rotation = Quaternion.Euler(tiltAngle, lookAngle, 0f);
    }
}
