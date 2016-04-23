using UnityEngine;
using System.Collections;

public class BounceCrate : JumpCrate
{
	public int bounces = 5;
	private int currentBounce;
	
	public override void OnJump ()
	{
		currentBounce++;

		if (currentBounce >= bounces) {
			InstanciaRewardInCrash ();
			Destroy ();
		} else {
			base.OnJump ();
			InstanciaRewardInCrash ();
		}
	}


	public override void OnHead ()
	{
		currentBounce++;
		
		if (currentBounce >= bounces) {
			InstanciaRewardInCrash ();
			DropTopCrate ();
			Destroy ();
		} else {
			base.OnJump ();
			InstanciaRewardInCrash ();
		}
	}

	private void InstanciaRewardInCrash ()
	{
		//Instantiate (rewardInstance, Platform3DActor.CentralPosition, 
		//             rewardInstance.transform.rotation);
	}
}
