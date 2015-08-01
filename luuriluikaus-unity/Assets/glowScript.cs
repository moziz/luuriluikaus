using UnityEngine;
using System.Collections;

public class glowScript : MonoBehaviour {
	
	Light myLight;

	// Use this for initialization
	void Start () {
		myLight = GetComponent<Light>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
		myLight.intensity = Mathf.PingPong(Time.time*0.2f, 0.5f) + 0.5f;
	
	}
}
