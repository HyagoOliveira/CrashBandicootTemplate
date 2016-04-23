using UnityEngine;
using System.Collections;

public class IKGenericGoal : IKHeadLook
{
	[SerializeField]
	private AvatarIKGoal
		goal = AvatarIKGoal.LeftHand;

	private Transform member;
	private Quaternion memberRoration;
	private float memberWeight = 0f;
	public Vector3 distancePos;
	private Vector3 contactPoint;

	public AvatarIKGoal Goal {
		get { return goal;}
		set {
			goal = value;
			switch (goal) {
			case AvatarIKGoal.LeftFoot:
				member = animator.GetBoneTransform (HumanBodyBones.LeftFoot);
				break;
				
			case AvatarIKGoal.RightFoot:
				member = animator.GetBoneTransform (HumanBodyBones.RightFoot);
				break;
				
			case AvatarIKGoal.LeftHand:
				member = animator.GetBoneTransform (HumanBodyBones.LeftHand);
				break;
				
			case AvatarIKGoal.RightHand:
				member = animator.GetBoneTransform (HumanBodyBones.RightHand);
				break;
			}
		}
	}

	public override void Start ()
	{
		base.Start ();
		Goal = goal;

		//fist raycast to find where is the contact point
		float distance = Vector3.Distance (member.position, goalObject.position);		
		Vector3 direction = (goalObject.position - member.position) / distance;
		
		RaycastHit hit;
		Physics.Raycast (member.position, direction, out hit, Mathf.Infinity);
		
		if (hit.transform != null) {
			distancePos = hit.point - goalObject.position;
		}
	}

	public override void OnAnimatorIK ()
	{
		if (goalObject != null) {
			animator.SetIKPosition (goal, contactPoint);
			animator.SetIKPositionWeight (goal, weight);			
			
			animator.SetIKRotation (goal, memberRoration);			
			animator.SetIKRotationWeight (goal, memberWeight);	
		}
	}

	public override void Update ()
	{
		base.Update ();
		contactPoint = goalObject.position + distancePos;

		DebugDraw.DrawMarker (contactPoint, 2f, Color.black, 0f, false);

		float distance = Vector3.Distance (member.position, contactPoint);		
		Vector3 direction = (contactPoint - member.position) / distance;

		RaycastHit hit;
		Physics.Raycast (member.position, direction, out hit, Mathf.Infinity);

		if (hit.transform != null) {
			DebugDraw.DrawVectorHit (member.position, direction, hit);

			//set rotation to match to surface's rotatation
			memberRoration = Quaternion.FromToRotation (Vector3.up, hit.normal);

			//set weight proporcionally to distance. Less distance, more weight
			memberWeight = selected_weight - Mathf.Clamp (distance, 0f, selected_weight);
		}
	}
}
