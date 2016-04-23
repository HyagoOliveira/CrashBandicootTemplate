using System;
using UnityEngine;

/// <summary>
/// Follow a 2.5D player.
/// Put this script inside a Camera and select the target.
/// </summary>
public class Follow2DCamera : FollowCamera
{
    public float snapDistance = 2.5f;
    public int maxSnaps = 3;

    private float _leftBoundary = float.MinValue;
    public float LeftBoundary { get { return _leftBoundary + 2f * currentSnap; } set { _leftBoundary = value; } }
    public float RightBoundary { get { return _rightBoundary - 2f * currentSnap; } set { _rightBoundary = value; } }
    private float _rightBoundary = float.MaxValue;

    private float initialForwardDistance = 0;

    private int currentSnap = 0;

    private Transform lastTarget;
    private Vector3 lastCenterOffet;
    private Platform2DActor playerActor;

    protected override void Start()
    {
        base.Start();

        playerActor = target.transform.root.GetComponent<Platform2DActor>();
        initialForwardDistance = centerOffset.z;
    }

    public override void ResetCameraPosition()
    {
        if (playerActor.input.GetButtonUp(resetButtonName))
        {
            currentSnap = (currentSnap + 1) % (maxSnaps + 1);
            centerOffset.z = initialForwardDistance - currentSnap * snapDistance;
        }
    }

    protected override void FollowTarget()
    {
        Vector3 position = target != null ? target.transform.position + centerOffset : centerOffset;
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * dampingSpeed);
        Vector3 clampedPos = transform.position;

        clampedPos.x = Mathf.Clamp(transform.position.x, LeftBoundary, RightBoundary);
        transform.position = clampedPos;
    }

    protected override void UpdateResetCamera()
    {
        throw new NotImplementedException();
    }

    public void SetForwardDistance(float distance)
    {
        centerOffset = new Vector3(centerOffset.x, centerOffset.y, distance);
    }

    public void Reset()
    {
        ResetForwardDistance();
        FreePosition();
    }

    public void ResetForwardDistance()
    {
        centerOffset = new Vector3(centerOffset.x, centerOffset.y, initialForwardDistance);
    }

    public void LockPosition(Vector3 position)
    {
        lastTarget = target.transform;
        lastCenterOffet = centerOffset;

        target = null;
        centerOffset = position;
    }

    public void FreePosition()
    {
        target.transform.position = lastTarget.position;
        centerOffset = lastCenterOffet;
    }

    public Vector3 GetTopLeftWorldPosition(float unitsAwayFromCamera)
    {
        return GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0f, 1f, unitsAwayFromCamera));
    }

    public Vector3 GetTopLeftWorldPosition()
    {
        return GetTopLeftWorldPosition(-transform.position.z);
    }

    public Vector3 GetBottomLeftWorldPosition(float unitsAwayFromCamera)
    {
        return GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0f, 0f, unitsAwayFromCamera));
    }

    protected override void UpdateCamera() { }

#if UNITY_EDITOR_WIN
    void OnDrawGizmos()
    {
        Vector3 leftBoundaryPosition = new Vector3(LeftBoundary, transform.position.y, transform.position.z);
        Vector3 rightBoundaryPosition = new Vector3(RightBoundary, transform.position.y, transform.position.z);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftBoundaryPosition, 1);
        Gizmos.DrawSphere(rightBoundaryPosition, 1);
        Gizmos.DrawLine(leftBoundaryPosition, rightBoundaryPosition);
    }
#endif
}
