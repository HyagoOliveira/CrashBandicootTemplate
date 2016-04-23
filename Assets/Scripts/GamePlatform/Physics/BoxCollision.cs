using UnityEngine;

public class BoxCollision : CheckCollision
{
    public Vector3 size = Vector3.one;

    public override void UpdateColliders()
	{
        Colliders = Physics.OverlapBox(Position, size / 2f, transform.rotation, contactLayer);
    }

    protected override void DrawGizmos()
	{
		Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Position, size);
    }
}