using UnityEngine;
using System;

/// <summary>
/// This script must be attached to a camera game object.
/// It'll fallow the target. If the target jumps, the camera won't fallow in the y direction.
/// </summary>

public class Static3DCameraController : Follow3DCamera
{
	public Vector2 cameraRange = Vector2.one * 10f;

	private float lastYPosition = 0f;	
	private float lookXAngle;                            // The rig's y axis rotation.
	private float tiltYAngle;                            // The pivot's x axis rotation.  

	private Vector2 angle;
	private Vector3 originalRotation;

	protected override void Start ()
	{
		base.Start ();
		angle = transform.eulerAngles;
		originalRotation = transform.eulerAngles;
	}

	protected override void FollowTarget ()
	{
		Vector3 newposition = target.transform.position + centerOffset;

		if (!target.Grounded)
			newposition.y = centerOffset.y + lastYPosition;
		else
			lastYPosition = target.transform.position.y;


		transform.position = Vector3.Lerp (transform.position, newposition, 
                                          Time.deltaTime * dampingSpeed);
	}


    protected override void UpdateCameraRotation()
    { 
		if (lookInput.sqrMagnitude > 0) {
			float speed = Time.deltaTime * rotationSmoothing;
			
			// smooth the user input
			smooth.x = Mathf.SmoothDamp (smooth.x, lookInput.x, ref smoothVelocity.x, speed);
			smooth.y = Mathf.SmoothDamp (smooth.y, lookInput.y, ref smoothVelocity.y, speed);
			
			angle.x += smooth.x * rotationSpeed; 
			angle.x = Mathf.Clamp (angle.x, originalRotation.x - cameraRange.x, cameraRange.x + originalRotation.x);
			
			angle.y += smooth.y * rotationSpeed; 
			angle.y = Mathf.Clamp (angle.y, originalRotation.y - cameraRange.y, cameraRange.y + originalRotation.y);
			
			transform.rotation = Quaternion.Euler (angle.y, angle.x, 0f);
		}
	}

	protected override void UpdateResetCamera ()
	{
		Quaternion newrotation = Quaternion.Slerp (
            transform.rotation, Quaternion.Euler (originalRotation),
            Time.deltaTime * rotationSpeed * 10f);
		transform.rotation = newrotation;

		if (Quaternion.Angle (newrotation, Quaternion.Euler (originalRotation)) < 1.5f) {
			resetCamera = false;
			angle = transform.eulerAngles;
		}
	}

    public override void ResetCameraPosition()
    {
        throw new NotImplementedException();
    }

    protected override void UpdateCamera()
    {
        throw new NotImplementedException();
    }
}
