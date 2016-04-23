using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BoxCollision))]
public abstract class PlatformActor : Actor, IKillable
{
    #region Attributes    
    public float jumpHeight = 12f;
    public float jumpExtendedMult = .5f;
    public float gravity = 20f;

    public bool canControl = true;
    public bool canJump = true;
    public bool enableDoubleJump = true;

    public Transform centralBone;

    public PlayerInput input { get; protected set; }
    public CharacterController characterController { get; protected set; }
    public BoxCollision footCollision {get; protected set;}    

    public bool Grounded { get; protected set; }
    public bool InAir { get { return !Grounded; } }
    public bool IsMoving { get { return hspeed > 0f; } }
    public bool Jumping { get { return StateController.CurrentActorState == PlatformState.JUMPING; } }
    public bool Falling { get { return StateController.CurrentActorState == PlatformState.FALLING || vspeed < 0f; } }

    //public bool Jumped { get; protected set; }

    //public ActorState State { get; protected set; }

    //public AudioProvider audioProvider { get; protected set; }
    //public PlayerPrefabProvider prefabProvider { get; protected set; }

    public Vector3 CentralPosition { get { return centralBone.position; } }  

    public bool OverSlope { get { return FloorAngle > 0.5f; } }

    public RaycastHit FarBottomHit
    {
        get { return _farBottomHit; }
        set { _farBottomHit = value; }
    }
    private RaycastHit _farBottomHit;


    public float FloorAngle { get { return Vector3.Angle(_farBottomHit.normal, Vector3.up); } }
    public float FloorDistance { get { return Vector3.Distance(transform.position, _farBottomHit.point); } }
    public float hspeedNormalized { get { return hspeed / speed; } }  
    public float overallSpeed { get { return characterController.velocity.magnitude; } }
    public float vspeed { get { return characterController.velocity.y; } }
    public float hspeed 
    {
        get 
        {
            Vector3 hvel = characterController.velocity;
            hvel.y = 0f;
            return Vector3.Dot (hvel, animatedMesh.forward); 
        }
    }    

    protected int lastLandedFrame = 0;    
    protected bool canDoDoubleJump;
    protected bool doubleJumping;
    //protected SphereCollision footSphere;    

    protected readonly int MIN_ALLOWED_FRAME_JUMP_COUNT = 4; //Player can jump again 4 frames after landed
    #endregion

    public override void SetupComponents()
    {
        input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator> ();
        StateController = GetComponent<ActorStateController>();
        characterController = GetComponent<CharacterController> ();
        footCollision = GetComponent<BoxCollision> ();

        //audioProvider = GetComponent<AudioProvider> ();
    }

    public override void SetupDefaultValues()
    {
        LookingDirection = moveDirection;
        canDoDoubleJump = enableDoubleJump;
        OriginalSpeed = speed;
    }

    protected virtual void Update()
    {
        if (canControl)
        {
            UpdateActor();
            UpdateHorizontalMovement();
        }

        UpdatePhysics();
        UpdateVerticalMovement();
        UpdateAnimation();
        UpdateMovement();
        UpdateStateMachine();
    }

    
    public override void UpdateAnimation()
    {
        animator.SetFloat("hSpeed", hspeedNormalized);
        animator.SetFloat("vSpeed", vspeed);
        animator.SetBool("ground", Grounded);
        animator.SetBool("jumping", Jumping);
        animator.SetBool("doubleJumping", doubleJumping);
    }

    public override void Kill()
    {
        OnKilled();
    }


    #region Jump/Land
    public virtual void ApplyJump()
    {
        if (input.GetButtonDown ("Jump") && Grounded && CanJump()) 
        {
            ApplyJumpImpulse ();
        }
        
        ApplyDoubleJump();
        ApplyExtendedJump();        
    }

    protected virtual void ApplyDoubleJump()
    {
        if (canDoDoubleJump && input.GetButtonDown("Jump") && !Grounded)
        {
            doubleJumping = true;
            canDoDoubleJump = false;
            ApplyJumpImpulse();
        }
    }

    protected virtual void ApplyExtendedJump()
    {
        if (input.GetButton ("Jump") && !Grounded && vspeed > 0f) 
        {
            moveDirection.y += jumpHeight * jumpExtendedMult * Time.deltaTime;
        }
    } 

    protected virtual bool CanJump()
    {
        return Time.frameCount - lastLandedFrame > MIN_ALLOWED_FRAME_JUMP_COUNT;
    }

    protected virtual void ApplyJumpImpulse ()
    {
        ApplyJumpAnimation ();
        moveDirection.y = jumpHeight;        
    }

    protected virtual void ApplyJumpAnimation ()
    {
        animator.SetTrigger ("jump");
    }

    protected virtual void ApplyLandLogic()
    {
        canDoDoubleJump = enableDoubleJump;
        doubleJumping = false;
        moveDirection.y = 0;
        lastLandedFrame = Time.frameCount;
        FixInGround();
        animator.ResetTrigger("jump");
    }
    #endregion

    #region Physics
    public override void UpdatePhysics()
    {
        if (!UpdateBottomRay() && vspeed <= -gravity) 
        {
            OnFallOffWorld ();
        }

        Grounded = footCollision.IsColliding ();
        UpdateSlopeLimit();

        #if UNITY_EDITOR_WIN
        //DebugDraw.DrawVector (footCollision.Position, Vector3.down, FarBottomHit.distance, .2f, Color.red, 0f, false);
        #endif
    }

    public bool UpdateBottomRay()
    {
        return Physics.Raycast(footCollision.Position, Vector3.down, out _farBottomHit,
            Mathf.Infinity, footCollision.contactLayer);
    }

    public override void FixInGround()
    {
        UpdateBottomRay();
        transform.position = FarBottomHit.point;// + Vector3.down;
    }

    public override void UpdateHorizontalMovement()
    {
        UpdateGroundedHorizontalMovement();
        if (canJump)
            ApplyJump();
    }

    public override void UpdateVerticalMovement()
    {
        //if player is on air
        if (InAir)
        {
            // and it has reach max vertical air speed (gravity)
            if (moveDirection.y > -gravity)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            else
            {
                moveDirection.y = -gravity;
            }            
        }

        //if it is falling and hit the floor (has landed)
        if (Falling && footCollision.IsColliding())
        {
            ApplyLandLogic();
        }
    }

    public override void Stop()
    {
        canControl = false;
        StopAllCoroutines();
        StopVelocity();
        input.Stop();
        gameObject.SetActive(false);
    }

    public override void StopVelocity()
    {
        moveDirection = Vector3.zero;
        UpdateMovement();
    }

    public override void StopVerticalVelocity()
    {
        moveDirection.y = 0f;
        UpdateMovement();
    }

    public override void StopHorizontalVelocity()
    {
        moveDirection.x = 0f;
        moveDirection.z = 0f;
        UpdateMovement();        
    }

    public override void OnFallOffWorld()
    {
        ApplyRespawn ();
    }

    public override void UpdateSlopeLimit()
    {
        if (OverSlope && IsMoving && FloorDistance < 0.1f)
        {
            RaycastHit hit2;
            if (Physics.SphereCast(footCollision.Position + Vector3.up, 
                0.01f, Vector3.down, out hit2, 
                Mathf.Infinity, footCollision.contactLayer))
            {
                DebugDraw.DrawMarker(hit2.point, 1f, Color.black, 0f);

                Vector3 pos = transform.position;
                pos.y = hit2.point.y;
                transform.position = pos;
                Grounded = true;
            }
        }
    }

    public override void UpdateMovement()
    {
        UpdateMovement(moveDirection);
    }

    //use this method when other GO needs to move player
    public override void UpdateMovement(Vector3 mov)
    {
        if (gameObject.activeInHierarchy)
        {
            characterController.Move(mov * Time.deltaTime);
        }
    }

    public override void ActivateMovement()
    {
        canControl = true;
    }

    public override void DeactivateMovement()
    {
        input.Stop();
        StopVelocity();
        canControl = false;
    }
    #endregion

    #region State Machine
    public override void LoadStates()
    {
        StateController.AddState(PlatformState.IDLE, true);
        StateController.AddState(PlatformState.RUNNING);
        StateController.AddState(PlatformState.JUMPING);
        StateController.AddState(PlatformState.FALLING);
    }

    public override void UpdateStateMachine()
    {
        if (Grounded)
        {
            UpdateGroundedStateMachine();
        }
        else
        {
            UpdateAirStateMachine();
        }

        StateController.ApplyChange();
    }

    public override void UpdateGroundedStateMachine()
    {
        if (hspeed > 0.1f)
        {
            StateController.ChangeState(PlatformState.RUNNING);
        }
        else
        {
            StateController.ChangeState(PlatformState.IDLE);
        }
    }

    public override void UpdateAirStateMachine()
    {
        if (vspeed > 0f)
        {
            StateController.ChangeState(PlatformState.JUMPING);
        }
        else
        {
            StateController.ChangeState(PlatformState.FALLING);
        }
    }
    #endregion

    public virtual void ApplyRespawn()
    {
        transform.position = RespawnPosition;
    }

    public virtual void OnKilled()
    {
        ApplyRespawn();
    }
}
