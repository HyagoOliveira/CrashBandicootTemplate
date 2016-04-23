using UnityEngine;
using System.Collections;
using Xft;

[RequireComponent(typeof(PlayerPrefabProvider))]
[RequireComponent(typeof(IKHeadLook))]
public class Crash : Platform3DActor, IHittable
{
    public SphereCollision headSphere;
    public SphereCollision spinSphere;
    public bool canSpin = true;
    public XWeaponTrail[] spinTrails;
    public float crawlingSpeed = 3f;

    private PlayerPrefabProvider prefabProvider;
    private IKHeadLook headLook;
    private bool spinning;
    private bool crouched;
    private bool crawling;
    private bool bellyFall;

    #region Override Methods   
    public override void Stop()
    {
        if (spinning)
        {
            StopSpin();
        }
        base.Stop();
    }

    public override void SetupComponents()
    {
        base.SetupComponents();
        //prefabProvider = GetComponent<PlayerPrefabProvider>();
        headLook = GetComponent<IKHeadLook>();
    }

    public override void LoadStates()
    {
        base.LoadStates();
        StateController.GetState(PlatformState.IDLE).RegisterOnState(StayIdle).
            RegisterOnExitState(ExitIdle);

        StateController.GetState(PlatformState.RUNNING).RegisterOnEnterState(StartRunning);
        StateController.GetState(PlatformState.JUMPING).RegisterOnState(CrateHeadJump);
        StateController.GetState(PlatformState.FALLING).RegisterOnState(CrateJump);

        StateController.AddState(CrashState.SPINNING).RegisterOnEnterState(StartSpinning);
        StateController.AddState(CrashState.CROUCHING).RegisterOnEnterState(StartCrouch).
            RegisterOnExitState(ExitCrouch);
        StateController.AddState(CrashState.CRAWLING).RegisterOnEnterState(StartCrawling).
            RegisterOnExitState(ExitCrawling);
        StateController.AddState(CrashState.BELLY_FALL).RegisterOnEnterState(StartBellyFall);

    }


    public override void UpdateActor()
    {
        //print(StateController.ToString());

        if (!spinning && canSpin && input.GetButtonDown("Attack") && !input.GetButtonDown("Jump"))
        {
            spinning = true;            
        }

        bellyFall = !spinning && InAir && input.GetButton("Crouch");
        crouched = !bellyFall && input.GetButton("Crouch");
        crawling = crouched && input.IsMoving;



        if (spinning)
        {
            //IHittable hittable = spinSphere.CollidingWith<IHittable>();
            //if (hittable != null)
            //{
            //    hittable.OnHit(centralPosition.position);
            //}
        }
        else
            CrateFallHeadBreak();

        //if (input.GetButtonDown("ShowInventory"))
        //    CrashInventory.Instance.ShowInventory();
    }


    public override void UpdateGroundedStateMachine()
    {
        base.UpdateGroundedStateMachine();
        //if (!spinning)
        //{
        //    if (hspeed > 0.1f)
        //        StateController.ChangeState("Running");
        //    else
        //        StateController.ChangeState("Idle");
        //}
    }

    public override void UpdateAirStateMachine()
    {
        base.UpdateAirStateMachine();

        //if (!spinning)
        //{
        //    if (vspeed > 0.1f)
        //        StateController.ChangeState("Jumping");
        //    else
        //        StateController.ChangeState("Falling");
        //}
    }

    //public override void UpdateAnimation()
    //{
    //if (!gameObject.activeInHierarchy)
    //    return;
    //base.UpdateAnimation();
    //animator.SetBool("spinning", spinning);
    //}

    protected override void ApplyLandLogic()
    {
        base.ApplyLandLogic();
        canSpin = true;
        if(StateController.CurrentActorState == CrashState.BELLY_FALL)
        {
            StopBellyFall();
        }
    }

    public override void OnKilled()
    {
        CrashInventory.Instance.RemoveLife();
        Stop();
        //respawn
        base.OnKilled();
    }

    public override void UpdateStateMachine()
    {
        base.UpdateStateMachine();
        if (spinning)
            StateController.ChangeState(CrashState.SPINNING);
        else if (crouched && !crawling)
            StateController.ChangeState(CrashState.CROUCHING);
        else if (crawling)
            StateController.ChangeState(CrashState.CRAWLING);
        else if (bellyFall)
            StateController.ChangeState(CrashState.BELLY_FALL);
    }

    #endregion

    #region State Machine Methods
    private void StartRunning()
    {
        print("começou a correr");
        //if (StateController.GetState("Idle").LastTime > 0.5f && hspeedNormalized > 0.2f)
        //    prefabProvider.InstanciateDecal("Smoke");
    }

    private void StayIdle()
    {
        if (input.IsMovingCamera)
        {
            headLook.Activate();
            headLook.destiveOverTime = false;
        }
        else
            headLook.destiveOverTime = true;

    }

    private void ExitIdle()
    {
        headLook.Desactivate();
    }

    private void StartSpinning()
    {
        print("entrou no spin");
        canDoDoubleJump = false;
        animator.ResetTrigger("spin");
        animator.SetTrigger("spin");
        spinning = true;

        if (Grounded)
            prefabProvider.Instanciate("Smoke");
        else
        {
            canSpin = false;
            animator.ResetTrigger("jump");
        }


        //activate spin trails
        for (int i = 0; i < spinTrails.Length; i++)
        {
            spinTrails[i].Activate();
        }

        StopCoroutine(StopSpinCorounine());
        StartCoroutine(StopSpinCorounine());
    }

    private void CrateJump()
    {
        print("dento do falling");
        //IJumpable jumpable = footSphere.CollidingWith<IJumpable>();
        //if (jumpable != null)
        //{
        //    ApplyJumpImpulse(jumpable.GetJumpHeight());
        //    jumpable.OnJump();
        //}
    }

    private void CrateHeadJump()
    {
        print("dentro do jump");
        //IJumpable jumpable = headSphere.CollidingWith<IJumpable>();

        //if (jumpable != null)
        //{
        //    jumpable.OnHead();
        //    StopVerticalVelocity();
        //}
    }

    private void StartCrouch()
    {
        print("Se abaixou");
    }

    private void ExitCrouch()
    {
        print("Se levantou");
    }

    private void StartCrawling()
    {
        print("Começou a engatinhar");
        speed = crawlingSpeed;
    }

    private void ExitCrawling()
    {
        print("Parou engatinhar");
        ResetSpeed();
    }

    private void StartBellyFall()
    {
        print("Belly Fall");
        DeactivateMovement();
        ApplyJumpImpulse();
    }

    private void StopBellyFall()
    {
        ActivateMovement();
    }
    #endregion

    private void CrateFallHeadBreak()
    {
        //Crate crate = headSphere.CollidingWith<Crate>();

        //if (crate != null && crate.Dropping)
        //{
        //    crate.OnHead();
        //}
    }




    void OnTriggerEnter(Collider other)
    {
        ITouchable collectable = other.gameObject.GetComponent<ITouchable>();
        if (collectable != null)
            collectable.OnTouch();
    }

    public void PlaySpinSound()
    {
        //if (Random.Range(0, 100) < 50)
        //    audioProvider.Play("crash_spin");
        //else
        //    audioProvider.Play("crash_spin_2");
    }

    private IEnumerator StopSpinCorounine()
    {
        yield return new WaitForSeconds(0.6f);
        StopSpin();
    }

    private void StopSpin()
    {
        animator.SetTrigger("spin-end");

        //desactivate spin trails
        for (int i = 0; i < spinTrails.Length; i++)
        {
            spinTrails[i].Deactivate();
        }

        spinning = false;

        if (Grounded)
            canDoDoubleJump = true;
    }

    #region IHittable implementation
    public void OnHit(Vector3 hitPosition)
    {
        Kill();
        prefabProvider.InstanciateDeathAnimation("Explosion");
    }

    public void OnHitByObject(Vector3 hitPosition)
    {
    }

    public void OnThrowAway()
    {
    }
    #endregion
}
