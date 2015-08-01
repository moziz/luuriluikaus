using UnityEngine;
using System.Collections;

public class runningSound : MonoBehaviour {

	GameObject myPlayer;
	PlayerCharacter myChar;
	AudioSource myAudio;

	// Use this for initialization
	void Start () {
		myPlayer = GameObject.Find("Player");
		myChar = myPlayer.GetComponent<PlayerCharacter>();
		myAudio = GetComponent<AudioSource>();
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if (myChar.currentSpeed > 0f && !myChar.isJumping){
			if (!myAudio.isPlaying){
				myAudio.Play();
			}
			
			myAudio.pitch = 1f + myChar.currentSpeed * 5.0f;
		
		}else{
			myAudio.Stop();
		}
	
	}
}
