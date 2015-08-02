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
	AudioSource rotaryAudio;
	
	public AudioClip[] throwYell;
	public AudioClip[] rotarySounds;
	
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
		rotaryAudio = transform.Find("rotary").GetComponent<AudioSource>();
		
		
		GameObject phone = GameObject.Find("Phone");
		if (phone)
		{
			PhoneController p = phone.GetComponent<PhoneController>();
			p.SubscribeOnRotaryRelease(NumberSelected);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		if(myChar.notStarted)
        {
            return;
        }
		
		// running sound
		if (myChar.currentSpeed > 0f && !myChar.isJumping && !myChar.gameEnding){
			if (!runningAudio.isPlaying){
				runningAudio.Play();
			}
			
			runningAudio.pitch = (1f + myChar.currentSpeed * 5.0f) * myChar.ownDeltaTime;
			
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
			
			if (myChar.CurrentlyThrowing && !throwingAudio.isPlaying && !volleyPlayed){
				//jumalautaAudio.clip = throwYell[Random.Range(0, throwYell.Length)];
				throwingAudio.Play();
				volleyPlayed = true;
			}
			
			if (!myChar.CurrentlyThrowing){
				volleyPlayed = false;
			}
			
		}else{
			throwPlayed = false;
		
		}
		
		//gameover
		if (myChar.gameEnding){
		
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
	
	void NumberSelected(int number)
	{
		if (number > 0){
		rotaryAudio.clip = rotarySounds[number-1];
		Debug.Log("number selected " + number);
		}else{
		rotaryAudio.clip = rotarySounds[9];
		}
		
		rotaryAudio.Play();
	}
}
