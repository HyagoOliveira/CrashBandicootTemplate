using UnityEngine;
using System.Collections;

public class RewardCrate : Crate
{
	public int finalRewards = 3;
	public GameObject rewardInstance;
	
	private readonly Vector2[] direction = new Vector2[5]
	{
		Vector2.zero,
		-Vector2.one,
		new Vector2 (1, -1),
		Vector2.one,
		new Vector2 (-1, 1)
	};

	
	public override void Destroy ()
	{
		CrateProvider.Instance.InstanciateBreakParticleSystem (transform);
		
		GiveReward ();
		Destroy (gameObject);
	}
	
	public virtual void GiveReward ()
	{
		if (rewardInstance == null)
			return; 
		
		float divFactor = rewardInstance.transform.localScale.sqrMagnitude / rewardInstance.transform.localScale.x;
		Vector3 originPosition = transform.position + Vector3.up * transform.localScale.y * 0.8f;
		
		for (int i = 0; i < finalRewards; i++) {
			
			int dirIndex = i % direction.Length;
			
			Vector3 position = originPosition + 
				Vector3.up * (i / direction.Length * rewardInstance.transform.localScale.y / divFactor) + //height
				transform.forward * direction [dirIndex].y * rewardInstance.transform.localScale.z / divFactor + //forward	
				transform.right * direction [dirIndex].x * rewardInstance.transform.localScale.x / divFactor;	//horizontal
			
			
			GameObject reward = Instantiate (rewardInstance, position, rewardInstance.transform.rotation) as GameObject;
			CollectableItem item = reward.GetComponent<CollectableItem> ();
			if (item != null) {
				if (throwRewardItemsAway) {
					item.OnHit (transform.position);
				} else {
					item.canThrowAway = false;
					item.EnableThrowAway ();
				}
			}
		}
	}    
}
