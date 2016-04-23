using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioProvider : IProvider<AudioClip>
{
	public AudioClip[] audioClips;

	public AudioSource audioSource{ get; set; }

	public override void Start ()
	{
		base.Start ();
		audioSource = GetComponent<AudioSource> ();
	}

	void Update ()
	{
		audioSource.pitch = Time.timeScale;
	}


	public override void Register ()
	{
		_cache = new Dictionary<string, AudioClip> ();
		
		for (int i = 0; i < audioClips.Length; i++) {
			_cache.Add (audioClips [i].name, audioClips [i]);
		}
	}

	public override AudioClip Get (string audioClipName)
	{
		if (_cache.ContainsKey (audioClipName))
			return _cache [audioClipName];
		
		throw new UnityException ("No " + audioClipName + " audio clip registered!");
	}

	public void Play (string audioClipName)
	{
		Play (audioClipName, 1f);
	}

	public void PlayRandom (string audioClipName)
	{
		Play (audioClipName, 1f);
	}

	public void Play (string audioClipName, float volume)
	{
		if (!audioSource.isPlaying && audioSource.isActiveAndEnabled)
			audioSource.PlayOneShot (Get (audioClipName), volume);
	}
}
