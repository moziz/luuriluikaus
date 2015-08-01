using UnityEngine;
using System.Collections;

public class glowScript : MonoBehaviour {
    public float blinkSpeed = 16f;
    
	Light myLight;
    float originalIntensity;
    float intensityDeltaMultiplier = 0.3f;
    
	// Use this for initialization
	void Start () {
		myLight = GetComponent<Light>();
        originalIntensity = myLight.intensity;
    }

    // Update is called once per frame
    void Update () {
		myLight.intensity = originalIntensity - Mathf.PingPong(blinkSpeed * Time.time + Random.value, originalIntensity * intensityDeltaMultiplier);
	}
}
