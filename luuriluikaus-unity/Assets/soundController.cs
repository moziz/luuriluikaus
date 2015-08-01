using UnityEngine;
using System.Collections;

public class soundController : MonoBehaviour {

	GameObject myPlayer;
	PlayerCharacter myChar;
	AudioSource runningAudio;
	AudioSource jumpingAudio;
	AudioSource throwingAudio;
	AudioSource jumalautaAudio;
	AudioSource shutdownAudio;
	AudioSource startAudio;
	AudioSource crashAudio;
	
	public AudioClip[] throwYell;
	
	bool jumpStarted = false;
	bool throwPlayed = false;
	bool volleyPlayed = false;
	bool shutdownPlayed = false;
	bool startPlayed = false;
	
	// Use this for initialization
	void Start () {
		myPlayer = GameObject.Find("Player");
		myChar = myPlayer.GetComponent<PlayerCharacter>();
		runningAudio = transform.Find("running").GetComponent<AudioSource>();
		jumpingAudio = transform.Find("jumping").GetComponent<AudioSource>();
		throwingAudio = transform.Find("throwing").GetComponent<AudioSource>();
		jumalautaAudio = transform.Find("jumalauta").GetComponent<AudioSource>();
		shutdownAudio = transform.Find("shutdown").GetComponent<AudioSource>();
		startAudio = transform.Find("start").GetComponent<AudioSource>();
		crashAudio = transform.Find("crash").GetComponent<AudioSource>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		// running sound
		if (myChar.currentSpeed > 0f && !myChar.isJumping && !myChar.gameOver){
			if (!runningAudio.isPlaying){
				runningAudio.Play();
			}
			
			runningAudio.pitch = 1f + myChar.currentSpeed * 5.0f;
			
		}else{
			runningAudio.Stop();
		}
		
		// jumpsound
		if (myChar.isJumping){
			if (!jumpingAudio.isPlaying && !jumpStarted){
				jumpingAudio.Play();
				jumpStarted = true;
			}
		
		}else{
			jumpStarted = false;
		}
		
		// throwsound
		if (myChar.hasThrown){
			if (!throwingAudio.isPlaying && !throwPlayed){
				//throwingAudio.Play();
				jumalautaAudio.clip = throwYell[Random.Range(0, throwYell.Length)];
				jumalautaAudio.Play();
				throwPlayed = true;
				volleyPlayed = true;
			}
			
			if (myChar.throwing && !throwingAudio.isPlaying && !volleyPlayed){
				//jumalautaAudio.clip = throwYell[Random.Range(0, throwYell.Length)];
				throwingAudio.Play();
				volleyPlayed = true;
			}
			
			if (!myChar.throwing){
				volleyPlayed = false;
			}
			
		}else{
			throwPlayed = false;
		
		}
		
		//gameover
		if (myChar.gameOver){
		
			if (!shutdownPlayed && !shutdownAudio.isPlaying){
				shutdownAudio.Play();
				crashAudio.Play();
				shutdownPlayed = true;
				startPlayed = false;
			}
			
		}else{
			shutdownPlayed = false;
			
			 // game start
			if (!startAudio.isPlaying && !startPlayed){
				startAudio.Play();
				startPlayed = true;
			}
		}
		
		
		
	}
}
