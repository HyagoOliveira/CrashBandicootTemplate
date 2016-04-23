using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollision))]
[RequireComponent(typeof(Animator))]
public abstract class ExplosionCrate : Crate
{
	protected SphereCollision explosionRadius;
	protected Animator anim;
	protected AudioSource audioSource;

	void Start ()
	{
		explosionRadius = GetComponent<SphereCollision> ();
		anim = GetComponent<Animator> ();
		audioSource = GetComponent<AudioSource> ();
	}

	public virtual void OnExplode (float time)
	{
		StartCoroutine (ExplodeCoroutine (time));
	}

	protected virtual void ApplyAnimation ()
	{
		anim.SetTrigger ("Bounce");
		audioSource.Play ();
	}


	public override void Destroy ()
	{
		gameObject.layer = LayerMask.NameToLayer ("Scenario");
		explosionRadius.UpdateColliders();
		for (int i = 0; i < explosionRadius.Colliders.Length; i++) {

			if (explosionRadius.Colliders [i] != null) {
				ExplosionCrate explosionCrate = explosionRadius.Colliders [i].gameObject.GetComponent<ExplosionCrate> ();

				
				if (explosionCrate != null) {
					explosionCrate.OnExplode (0.2f);
				} else {
					IHittable hittable = explosionRadius.Colliders [i].gameObject.GetComponent<IHittable> ();
					if (hittable != null) {
						hittable.OnThrowAway ();
						hittable.OnHit (transform.position);
					}
				}
			}
		}	

		Destroy (gameObject);
	}

	private IEnumerator ExplodeCoroutine (float time)
	{
		yield return new WaitForSeconds (time);
		Destroy ();
	}

}
