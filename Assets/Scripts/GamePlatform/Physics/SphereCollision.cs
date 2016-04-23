using UnityEngine;

public class SphereCollision : CheckCollision
{
	public float radius = 1f;
    
	public override void UpdateColliders()
	{
		Colliders = Physics.OverlapSphere (Position, radius, contactLayer);
	}

	protected override void DrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (Position, radius);
	}
}