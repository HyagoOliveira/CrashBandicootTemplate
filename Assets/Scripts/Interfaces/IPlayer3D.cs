using UnityEngine;
using System.Collections;

public interface IPlayer3D
{
	void SetDefaultValues ();
	void LoadPlatformStates ();
	void UpdateActor ();
	void ApplyHorizontalMovement ();
	void UpdatePhysics ();
	void ApplyUpdateStateMachine ();
	void ApplyUpdateGroundedStateMachine ();
	void SlopeLimit ();
	void ApplyGroundedHorizontalMovement ();
	void ApplyVerticalMovement ();
	void ApplyMovement ();
	void ApplyMeshRotation ();
	void ApplyJump ();
	void OnFellOffWorld ();
	void ApplyAnimation ();
	void ApplyRespawn ();
}
