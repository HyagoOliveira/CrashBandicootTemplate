using UnityEngine;

public abstract class CheckCollision : MonoBehaviour
{
	public LayerMask contactLayer;
	public Transform targetTransform;
	public Collider[] Colliders;
	public Vector3 offset;

	public virtual Collider CollisionCollider { get{ return Colliders [currentCollisionColliderIndex];}	}
	public virtual Vector3 Position 
	{
		get 
		{
			if (targetTransform == null)
			{
				return transform.position + offset;
			}

			return targetTransform.position + offset;
		}
	}

	protected int currentCollisionColliderIndex = 0;
    
	private void Update()
	{
		UpdateColliders();
	}

    void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }

	public abstract void UpdateColliders();
	protected abstract void DrawGizmos();


	public virtual bool IsColliding ()
	{
		return CollidingCount() > 0;
	}

	public virtual int CollidingCount ()
	{
		return Colliders.Length;
	}

	public virtual bool IsCollidingWith<T> ()
	{
		for (int i = 0; i < Colliders.Length; i++) {
			T component = Colliders [i].gameObject.GetComponent<T> ();
			if (component != null) {
				currentCollisionColliderIndex = i;
				return true;
			}
		}

		return false;
	}

	public virtual T CollidingWith<T> ()
	{
		for (int i = 0; i < Colliders.Length; i++) {
			if (Colliders [i] != null) {
				T component = Colliders [i].gameObject.GetComponent<T> ();
				if (component != null) {
					currentCollisionColliderIndex = i;
					return component;
				}
			}
		}
		
		return default (T);
	}
}