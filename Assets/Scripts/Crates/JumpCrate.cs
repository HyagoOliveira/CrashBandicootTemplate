using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(AudioSource))]
public class JumpCrate : RewardCrate
{
	public float jumpHeight = 10f;
	protected Animation anim;
	protected AudioSource audioSource;

	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();
		anim = GetComponent<Animation> ();
	}
    
	public override void OnJump ()
	{
		anim.Play ();
		audioSource.Play ();
	} 

	public override float GetJumpHeight ()
	{
		return jumpHeight;
	}
}
