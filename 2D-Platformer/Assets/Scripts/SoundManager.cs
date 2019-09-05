using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	[SerializeField] AudioClip bulletFireSound;
	[SerializeField] AudioClip spawnSound;
	[SerializeField] AudioClip wallHitSound;
	[SerializeField] AudioClip warpSound;
	[SerializeField] [Range(0, 1)] float BGMVolume = 0.6f;
	[SerializeField] [Range(0,1)] float masterVolume = 1;

	private AudioSource audioSource;

	void Start()
    {
		audioSource = GetComponent<AudioSource>();
    }

	private void Update() {
		audioSource.volume = BGMVolume * masterVolume;
	}

	// Start is called before the first frame update
	public void PlayBulletFireSound() {
		audioSource.PlayOneShot(bulletFireSound, (0.8f * masterVolume));
	}

	public void PlaySpawnSound() {
		audioSource.PlayOneShot(spawnSound, (0.7f * masterVolume));
	}

	public void PlayWallHitSound() {
		audioSource.PlayOneShot(wallHitSound, (0.8f * masterVolume));
	}

	public void PlayWarpSound() {
		audioSource.PlayOneShot(warpSound, (0.7f * masterVolume));
	}
}
