using UnityEngine;

/// <summary>
/// Abstract class for fallow camera.
/// Camera may be controlled using mouse, gamepad or both
/// </summary>
public abstract class FollowCamera : MonoBehaviour
{
    public PlatformActor target;    
    public bool followTarget = true;
    public float dampingSpeed = 5f;   // speed when following target
    public string resetButtonName = "Reset Camera";

    protected Vector3 centerOffset = Vector3.zero;
    protected Vector3 originalcenterOffset;
    protected Vector2 lookInput;

    protected virtual void Start()
    {
        centerOffset = transform.position - target.transform.position;
        originalcenterOffset = centerOffset;        
    }

    public virtual void LateUpdate()
    {
        if (!followTarget)
            return;        

        GetInput();
        FollowTarget();
        UpdateCamera();
    }

    protected virtual void GetInput()
    {
        //lookInput = Vector2.zero;
        //lookInput += target.input.LookInput;
        lookInput = target.input.LookInput;

        if (target.input.GetButtonDown(resetButtonName))
        {
            ResetCameraPosition();
        }
    }

    public abstract void ResetCameraPosition();
    protected abstract void FollowTarget();
    protected abstract void UpdateCamera();
    protected abstract void UpdateResetCamera();
}

