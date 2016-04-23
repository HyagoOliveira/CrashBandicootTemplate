using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class Explosion : MonoBehaviour
{
	Light lightSource;

	// Use this for initialization
	void Start ()
	{
		lightSource = GetComponent<Light> ();
		Destroy (gameObject, 6f);	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (lightSource.intensity > 0)
			lightSource.intensity -= Time.deltaTime * 0.5f;
	}
}
