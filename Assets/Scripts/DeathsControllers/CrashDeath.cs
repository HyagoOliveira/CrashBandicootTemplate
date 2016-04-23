using UnityEngine;
using System.Collections;

public class CrashDeath : MonoBehaviour, IPlayerDeath
{
	public float deathTime = 5f;

	void Start ()
	{
		FixInGround ();
		FaceToCamera ();
		StartAnimation ();
	}

	void FixInGround ()
	{
		LayerMask floor = LayerMask.NameToLayer ("Floor");	

		foreach (RaycastHit h in Physics.RaycastAll (transform.position + Vector3.up * 10f, Vector3.down, 1000f)) {
			if (h.transform.gameObject.layer == floor) {
				transform.position = h.point;
				break;
			}
		}
	}

	void FaceToCamera ()
	{
		Vector3 cameraPosition = Camera.main.transform.rotation * Vector3.back;
		cameraPosition.y = 0f;
		transform.LookAt (transform.position + cameraPosition);
	}

	#region IPlayerDeath implementation

	public virtual void StartAnimation ()
	{
		Invoke ("StopAnimation", deathTime);
	}

	public virtual void StopAnimation ()
	{
		//Crash.Instance.ApplyRespawn ();
		Destroy (gameObject);
	}

	#endregion

}
