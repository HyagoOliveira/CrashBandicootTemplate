using UnityEngine;

public interface IHittable
{
	void OnHit (Vector3 hitPosition);
	void OnHitByObject (Vector3 hitPosition);
	void OnThrowAway ();
}
