using UnityEngine;
using System.Collections;

public class Decal : MonoBehaviour
{
	public float skin = 0.1f;
	public float timeAlive = 5f;
	public float randomScaleFactor = 4f;
	public LayerMask layer;

	private Renderer render;

	// Use this for initialization
	void Start ()
	{
		FixInGround ();
		render = GetComponent<Renderer> ();

		enabled = false;
		Invoke ("Enable", timeAlive);

	}

	private void Enable ()
	{
		enabled = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (render.material.HasProperty ("_InvFade")) {
			float f = render.material.GetFloat ("_InvFade");
			f -= Time.deltaTime;

			if (f < 0f) {
				Destroy (gameObject);
				return;
			}

			render.material.SetFloat ("_InvFade", f);
		}
	}

	private void FixInGround ()
	{		
		RaycastHit hit;
		if (Physics.Raycast (transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, layer)) {
			transform.rotation = Quaternion.LookRotation (hit.normal);

			transform.position = hit.point + Vector3.up * skin; 			
		}

		float scale = (transform.localScale.x + transform.localScale.y) / 2f;
		float scaleFactor = Random.Range (scale, scale + randomScaleFactor);

		transform.localScale = Vector3.one * scaleFactor;

		transform.eulerAngles += Vector3.forward * Random.Range (0f, 360f);
	}
}
