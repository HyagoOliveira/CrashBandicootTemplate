using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ActorStateController))]
public abstract class Actor : MonoBehaviour, IPauseable, IRespawnable, IMoveable
{
    #region Attributes
    public float speed = 4f;
    public float maxSpeed = 4f;
    public float rotateSpeed = 15f;

    public Transform animatedMesh;

    public Animator animator { get; protected set; }
    public ActorStateController StateController { get; protected set; }

    public Vector3 LookingDirection { get; protected set; }
    public Vector3 RespawnPosition { get; private set; }
    public Vector3 BoundsSize { get { return animatedMesh.GetComponent<Renderer>().bounds.size; } }

    public float GetWidth { get { return BoundsSize.x; } }
    public float GetHeight { get { return BoundsSize.y; } }
    public float GetLength { get { return BoundsSize.z; } }

    public float OriginalSpeed { get; protected set; }
        
    protected Vector3 moveDirection = Vector3.zero;
    #endregion
    

    protected virtual void Start()
    {
        SetupComponents();
        SetupDefaultValues();
        LoadStates();
        FixInGround();
        RespawnPosition = transform.position;
    }

    public abstract void SetupComponents();
    public abstract void SetupDefaultValues();
    public abstract void LoadStates();  
    public abstract void UpdateActor();
    public abstract void UpdatePhysics();
    public abstract void UpdateMovement();
    public abstract void UpdateMovement(Vector3 mov);
    public abstract void UpdateVerticalMovement();
    public abstract void UpdateHorizontalMovement();
    public abstract void UpdateGroundedHorizontalMovement();
    public abstract void UpdateStateMachine();
    public abstract void UpdateGroundedStateMachine();
    public abstract void UpdateAirStateMachine();
    public abstract void UpdateAnimation();
    public abstract void UpdateGroundedAnimation();
    public abstract void UpdateMeshRotation();
    public abstract void UpdateSlopeLimit();
    public abstract void FixInGround();
    public abstract void Stop();
    public abstract void StopVelocity();
    public abstract void StopVerticalVelocity();
    public abstract void StopHorizontalVelocity();
    public abstract void DeactivateMovement();
    public abstract void ActivateMovement();
    public abstract void Kill();

    public abstract void OnPause();
    public abstract void OnResume();
    public abstract void OnRespawn();
    public abstract void OnFallOffWorld();

    public virtual void ResetSpeed()
    {
        speed = OriginalSpeed;
    }
}
