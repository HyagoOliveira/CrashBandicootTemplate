using UnityEngine;
using System.Collections;


public class Checkbox : Crate
{	
	private Animator anim;
	private AudioSource audioSource;

	public override void OnHit (Vector3 hitPosition)
	{
		Open ();
	}

	public override void OnDropStop (Transform objectBellow)
	{
		base.OnDropStop (objectBellow);

		if (!objectBellow.gameObject.layer.Equals (LayerMask.NameToLayer ("Crates")))
			Open ();
	}

	public override void OnHitByObject (Vector3 hitPosition)
	{
		Open ();
	}

	public override void OnJump ()
	{
		Open ();
	}

	public override void OnHead ()
	{
		Open ();
	}

	private void Open ()
	{
		//Platform3DActor.Instance.RespawnPosition = transform.position;
		gameObject.layer = LayerMask.NameToLayer ("Floor");


		anim = GetComponent<Animator> ();
		anim.SetTrigger ("open");

		audioSource = GetComponent<AudioSource> ();
		audioSource.Play ();

		BoxCollider box = GetComponent<BoxCollider> ();
		box.center = new Vector3 (0.042f, 0.15f, 0.05f);
		box.size = new Vector3 (1.26f, 0.32f, 1.26f);

		//For otimization
		CrateProvider.Instance.DestroyComponents (2f, anim, audioSource);
		Destroy (this);
	}
}
