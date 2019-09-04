using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
	[Header("Camera Shake")]
	[SerializeField] [Range(0, 1)] float shakeDuration = 0.2f;
	[SerializeField] [Range(0, 2)] float shakeAmplitude = 1f;
	[SerializeField] [Range(0, 2)] float shakeFrequency = 1f;

	private float shakeTimer;
	private CinemachineVirtualCamera virtualCamera;
	private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
	// Start is called before the first frame update
	void Start()
    {
		//Get noise system
		virtualCamera = GetComponent<CinemachineVirtualCamera>();
		virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        if(shakeTimer > 0) {
			shakeTimer -= Time.deltaTime;
		}

		else {
			StopCameraShake();
		}
    }

	public void StartCameraShake() {
		shakeTimer = shakeDuration;
		virtualCameraNoise.m_AmplitudeGain = shakeAmplitude;
		virtualCameraNoise.m_FrequencyGain = shakeFrequency;
	}

	private void StopCameraShake() {
		virtualCameraNoise.m_AmplitudeGain = 0f;
		virtualCameraNoise.m_FrequencyGain = 0f;
	}
}
