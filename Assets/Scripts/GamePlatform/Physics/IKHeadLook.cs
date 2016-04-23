using UnityEngine;
using System.Collections;

public class IKHeadLook : MonoBehaviour
{
	protected Animator animator;

	public float weight = 0.8f;
	public float weightRate = 0.2f;
	public float activeTime = 2f;
	public Transform goalObject;
	public bool destiveOverTime;

	private bool disabling;
	private bool enabling;

	public float CurentActiveTime { get; protected set; }
	public bool IkActive { get; protected set; }

	protected float selected_weight;


	public virtual void Start ()
	{
		selected_weight = weight;
		IkActive = true;
		animator = GetComponent<Animator> ();	
	}	

	public virtual void OnAnimatorIK ()
	{
		if (goalObject != null) {
			animator.SetLookAtWeight (weight);
			animator.SetLookAtPosition (goalObject.position);			
		}
	}

	public virtual void Update ()
	{
		if (destiveOverTime && IkActive) {
			CurentActiveTime += Time.deltaTime;
			if (CurentActiveTime >= activeTime) {
				IkActive = false;
				CurentActiveTime = 0f;
				disabling = true;
			}
		}

		if (disabling) {
			weight -= weightRate * Time.deltaTime;
			if (weight <= 0f) {
				weight = 0f;
				disabling = false;
			}
		} else if (enabling) {
			weight += weightRate * Time.deltaTime;
			if (weight >= selected_weight) {
				weight = selected_weight;
				enabling = false;
			}
		}
	}

	public void Activate ()
	{
		if (IkActive)
			return;

		CurentActiveTime = 0f;
		weight = 0f;
		IkActive = true;
		enabling = true;
		disabling = false;
	}

	public void Desactivate ()
	{
		if (!IkActive)
			return;

		CurentActiveTime = 0f;
		weight = 1f;
		IkActive = false;
		enabling = false;
		disabling = true;
	}
}
