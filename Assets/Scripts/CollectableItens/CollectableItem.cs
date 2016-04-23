using UnityEngine;
using System.Collections;


public enum CollectableItemType
{
	WumpaFruit,
	AkuAku,
	Life,
	Gem,
	Crystal
}

public class CollectableItem : MonoBehaviour, ITouchable, IHittable, IDestructible
{
	public CollectableItemType type;
	public bool canThrowAway = true;
	public bool pull = true;
	public float pullRadius = 1f;
	public float rotSpeed = 50f;	
    
	private bool gone = false;
	private Vector3 outDirection;
	private float pullForce = 0f;

	void Update ()
	{
		if (gone) {
			transform.position += outDirection * 80f * Time.deltaTime;
		} else if (pull) {

			//horizontal distance
			//float distance = Vector3.Distance (Platform3DActor.centralBone.position, transform.position);

			//if (distance < pullRadius) {
			//	//set weight proporcionally to distance. Less distance, more weight
			//	pullForce = (pullRadius - Mathf.Clamp (distance, 0f, pullRadius)) * 10f;

			//	transform.position = Vector3.Slerp (transform.position, Platform3DActor.CentralPosition, 
			//                                    pullForce * Time.deltaTime);
			//}

		}

		if (!gone)
			transform.Rotate (Vector3.up * rotSpeed * Time.deltaTime, Space.World);
	}

	#region IHittable implementation

	public virtual void OnThrowAway ()
	{
		//Platform3DActor.Instance.audioProvider.Play ("item_hit");
		CrateProvider.Instance.InstanciateHitSparkParticleSystem (transform.position);
		
		Destroy (gameObject, 3f);
		gone = true;
	}
	
	public virtual void OnHitByObject (Vector3 origin)
	{
		throw new UnityException ("Can not be hit by other objects.");
	}

	public virtual void OnHit (Vector3 playerPosition)
	{
		if (!canThrowAway)
			return;
		
		outDirection = (transform.position - playerPosition).normalized;
		outDirection.y = 0f;
		if (outDirection.sqrMagnitude < 0.5f) {
			outDirection.x = Mathf.Sign (outDirection.x);
			outDirection.z = Mathf.Sign (outDirection.z);
		}
		
		
		OnThrowAway ();
	}
	
	#endregion



	public virtual void OnTouch ()
	{
		if (gone)
			return;

		Collect ();    	
	}



	public virtual void Collect ()
	{
		CrashInventory.Instance.AddItem (type);
		GameObject instance = Instantiate (CrateProvider.Instance.Get ("WumpaEatSpark"), transform.position,
		                                  Quaternion.identity) as GameObject;

		Destroy (instance, 4f);
		Destroy ();
	}

	public void EnableThrowAway ()
	{
		StartCoroutine (EnableThrowAwayCoroutine ());
	}

	#region IDestructible implementation

	public virtual void Destroy ()
	{
		Destroy (gameObject);
	}

	#endregion

	private IEnumerator EnableThrowAwayCoroutine ()
	{
		yield return new WaitForSeconds (1f);
		canThrowAway = true;
	}


#if UNITY_EDITOR
	void OnDrawGizmosSelected ()
	{
		if (pull) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (transform.position, pullRadius);
		}
	}
#endif
}
