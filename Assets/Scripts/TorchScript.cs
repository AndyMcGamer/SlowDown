using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchScript : MonoBehaviour
{
    [SerializeField] private Light2D flicker;
    [SerializeField] private float minIntensity, maxIntensity;
    [SerializeField] private float minInterval, maxInterval;
    [SerializeField] private float smoothSpeed;
    private float flickerTime;

    private void Update()
    {
        flickerTime -= Time.deltaTime;
        if(flickerTime <= 0)
        {
            float intensity = Random.Range(minIntensity, maxIntensity);
            flicker.intensity = Mathf.Lerp(flicker.intensity, intensity, smoothSpeed);
            flickerTime = Random.Range(minInterval, maxInterval);
        }
    }

}
