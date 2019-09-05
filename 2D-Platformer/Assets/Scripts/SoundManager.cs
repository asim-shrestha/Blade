using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	[SerializeField] AudioClip bulletFireSound;
	[SerializeField] AudioClip wallHitSound;
	[SerializeField] AudioClip warpSound;

	private AudioSource audioSource;

	void Start()
    {
		audioSource = GetComponent<AudioSource>();
    }

	// Start is called before the first frame update
	public void PlayBulletFireSound() {
		audioSource.PlayOneShot(bulletFireSound, 0.7f);
	}

	public void PlayWallHitSound() {
		audioSource.PlayOneShot(wallHitSound, 0.7f);
	}

	public void PlayWarpSound() {
		audioSource.PlayOneShot(warpSound, 0.45f);
	}
}
