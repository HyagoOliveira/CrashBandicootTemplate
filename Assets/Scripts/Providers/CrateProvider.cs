using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrateProvider : IProvider<GameObject>
{
	public GameObject[] breaksParticleSystem;
	public LayerMask decalLayer;
	public static CrateProvider Instance { get; private set; }

	public override void Start ()
	{
		base.Start ();
		
		if (Instance == null)
			Instance = this;
	}

	public void DestroyComponents (float time, params Component[] components)
	{
		for (int i = 0; i < components.Length; i++) {
			Destroy (components [i], time);
		}
	}


	public void InstanciateHitSparkParticleSystem (Vector3 position)
	{
		GameObject hitSpark = Get ("HitSpark");
		
		GameObject instance = GameObject.Instantiate (hitSpark, position, hitSpark.transform.rotation) as GameObject;
		Destroy (instance, 1f);
	}

	public void InstanciateBreakParticleSystem (Transform transform)
	{
		GameObject crateBreak = Get ("CrateBreak");

		GameObject instance = GameObject.Instantiate (crateBreak, transform.position, crateBreak.transform.rotation) as GameObject;
		Destroy (instance, 5f);
	}

	public void InstanciateTNTExplosionParticleSystem (Transform transform)
	{
		GameObject tntExplosion = Get ("TntExplosion");
		
		GameObject.Instantiate (tntExplosion, transform.position + Vector3.up * transform.localScale.y * 0.5f, 
		                        tntExplosion.transform.rotation);

		InstanciateRandomExplosionDecal (transform.position);
	}

	public void InstanciateNitroExplosionParticleSystem (Transform transform)
	{
		GameObject nitroExplosion = Get ("NitroExplosion");
		
		GameObject.Instantiate (nitroExplosion, transform.position + Vector3.up * transform.localScale.y * 0.5f, 
		                        nitroExplosion.transform.rotation);

		InstanciateRandomExplosionDecal (transform.position);
	}

	public void InstanciateRandomExplosionDecal (Vector3 position)
	{
		if (Physics.Raycast (position + Vector3.up * 0.5f, Vector3.down, 0.5f, decalLayer)) {

			if (Random.Range (0, 100) > 50)
				InstanciateDecal ("ExplosionDecalA", position);
			else
				InstanciateDecal ("ExplosionDecalB", position);
		}
	}

	public void InstanciateDecal (string prefabname, Vector3 position)
	{
		GameObject prefab = Get (prefabname);		
		GameObject.Instantiate (prefab, position, prefab.transform.rotation);
	}

	#region implemented abstract members of IProvider
	public override void Register ()
	{
		_cache = new Dictionary<string, GameObject> ();
		
		for (int i = 0; i < breaksParticleSystem.Length; i++) {
			_cache.Add (breaksParticleSystem [i].name, breaksParticleSystem [i]);
		}
	}

	public override GameObject Get (string name)
	{
		if (_cache.ContainsKey (name))
			return _cache [name];
		
		throw new UnityException ("No " + name + " registered!");
	}
	#endregion
}
