using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public abstract class Crate : MonoBehaviour,  IJumpable, IHittable, IDestructible
{
	public bool canDrop = true;

	protected bool throwRewardItemsAway;
	public bool Dropping{ get; protected set; }



	public virtual void Update ()
	{
		if (Dropping) {

			RaycastHit hit;
			if (Physics.Raycast (transform.position, Vector3.down, out hit, 0.3f, 
			                     LayerMask.GetMask ("Crates", "Floor"))) {
				Crate crate = hit.transform.GetComponent<Crate> ();

				if (crate == null) {
					transform.position = hit.point;
					OnDropStop (hit.transform);
				} else if (!crate.Dropping) {
					transform.position = hit.point;
					OnDropStop (hit.transform);
					crate.OnHitByDroppedObject ();
				}
			} 

		}
	}



	#region hittable interface methods
	public virtual void OnHit (Vector3 hitPosition)
	{
		DropTopCrate ();
		Destroy ();
	}

	public virtual void OnHitByObject (Vector3 hitPosition)
	{
		OnHit (hitPosition);
	}

	public virtual void OnThrowAway ()
	{
		throwRewardItemsAway = true;
	}
	#endregion

	#region jumpable interface methods
	public virtual void OnJump ()
	{
		Destroy ();
	}

	public virtual void OnHead ()
	{
		DropTopCrate ();
		Destroy ();
	}

	public virtual float GetJumpHeight ()
	{
        //returns default crash's jump height
        return 0;// Crash.Instance.jumpHeight;
	}
	#endregion

	#region Crates methods
	public virtual void DropTopCrate ()
	{
		Dropping = true;
		Vector3 origin = transform.position + Vector3.up * transform.localScale.y * 0.5f;

		RaycastHit hit;
		if (Physics.Raycast (origin, Vector3.up, out hit, transform.localScale.y)) {			
			Crate nextCrate = hit.transform.GetComponent<Crate> ();
			
			if (nextCrate != null && nextCrate.canDrop) {
				Rigidbody rigidbody = hit.transform.gameObject.GetComponent<Rigidbody> ();
				if (rigidbody == null)
					rigidbody = hit.transform.gameObject.AddComponent<Rigidbody> ();

				rigidbody.constraints = RigidbodyConstraints.FreezePositionX | 
					RigidbodyConstraints.FreezePositionZ |
					RigidbodyConstraints.FreezeRotation;

				nextCrate.Dropping = true;
				nextCrate.OnDropStart ();
				nextCrate.DropTopCrate ();
			}
		}
	}

	public virtual void OnHitByDroppedObject ()
	{
	}
		
	public virtual void OnDropStart ()
	{
	}

	public virtual void OnDropStop (Transform objectBellow)
	{		
		Dropping = false;
		Destroy (GetComponent<Rigidbody> ());
	}	

	#endregion

	public virtual void Destroy ()
	{
		throw new UnityException ("Each crate has its own destroy prefab!");
	}
}
