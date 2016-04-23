using UnityEngine;
using System.Collections;


public class TNT : ExplosionCrate
{
	private bool activated;

	private Renderer tntSideRenderer;
	private int count = 4;  

	#if UNITY_EDITOR_WIN
	public bool activateNow;

	public override void Update ()
	{
		base.Update ();
		if (activateNow) {
			StartCountDown ();
		}
	}
	#endif

	#region crate methods
	public override void OnHead ()
	{
		StartCountDown ();
	}

	public override void OnJump ()
	{
		StartCountDown ();
	}

	public override float GetJumpHeight ()
	{
		if (activated)
			return 0f;
		
		return base.GetJumpHeight ();
	}

	public override void Destroy ()
	{
		CrateProvider.Instance.InstanciateTNTExplosionParticleSystem (transform);		
		base.Destroy ();
	}

	#endregion

	#region hittable methods
	public override void OnHitByObject (Vector3 hitPosition)
	{
		StartCountDown ();
	}

	public override void OnDropStop (Transform objectBelow)
	{
		base.OnDropStop (objectBelow);
		StartCountDown ();
	}
	#endregion

	private void StartCountDown ()
	{
		if (activated)
			return;

		tntSideRenderer = GetComponent<Renderer> ();

		CountDown ();
		StartCoroutine (CountDownCorroutine (1f));
		StartCoroutine (CountDownCorroutine (2f));
		StartCoroutine (CountDownCorroutine (3f));
		
		activated = true;
	}


	private void CountDown ()
	{
		count--;
		
		if (count == 0) {
			Destroy ();
			return;
		}


		tntSideRenderer.materials [1].mainTexture = TextureProvider.Instance.Get ("crate_tnt_" + count);

		ApplyAnimation ();

		if (count == 1)
			StartCoroutine (SoundCorroutine ());
	}

	IEnumerator CountDownCorroutine (float time)
	{
		yield return new WaitForSeconds (time);
		CountDown ();
	}

	IEnumerator SoundCorroutine ()
	{
		yield return new WaitForSeconds (0.5f);
		ApplyAnimation ();
	}
}
