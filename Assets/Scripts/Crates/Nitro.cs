using UnityEngine;
using System.Collections;

public class Nitro : ExplosionCrate, ITouchable
{
	public bool bounce;
	public float bounceTime = 5f;

	private float currentBounceTime = 0;

	public override void Update ()
	{
		base.Update ();

		if (bounce) {
			currentBounceTime += Time.deltaTime;
			if (currentBounceTime > bounceTime) {
				currentBounceTime = 0f;
				ApplyAnimation ();
			}
		}
	}

	void OnTriggerEnter (Collider other)
	{
		ExplosionCrate explosionCrate = other.gameObject.GetComponent<ExplosionCrate> ();
		if (explosionCrate != null) {
			explosionCrate.Destroy ();
			Destroy ();
		}
	}

	public void OnTouch ()
	{
		Destroy ();
	}

	public override void OnHead ()
	{
		Destroy ();
	}

	public override void OnHitByObject (Vector3 hitPosition)
	{
		Destroy ();
	}

	public override void OnDropStart ()
	{
		Collider col = GetComponent<Collider> ();
		col.isTrigger = false;
	}

	public override void OnDropStop (Transform objectBelow)
	{
		base.OnDropStop (objectBelow);
		Destroy ();
	}

	public override void OnHitByDroppedObject ()
	{
		Destroy ();
	}

	public override void OnHit (Vector3 hitPosition)
	{
		Destroy ();
	}

	public override void Destroy ()
	{
		CrateProvider.Instance.InstanciateNitroExplosionParticleSystem (transform);
		base.Destroy ();
	}

	public void Collect ()
	{
		throw new UnityException ("Can not collect Nitros");
	}
}
