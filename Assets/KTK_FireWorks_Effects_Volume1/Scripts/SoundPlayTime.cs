using UnityEngine;
using System.Collections;

public class SoundPlayTime : MonoBehaviour {

	public bool playOnAwake = false;
	public bool loopCheck = false;
	private float timer;
	public float waitTime = 1.0f;
	private float loopTime = 0.0f;
	public float loopWaitTime = 0.0f;
	private bool flag = false;
	public  AudioClip sound01;
	private AudioSource audioSource;
	

	void Start  () {
		loopTime = loopWaitTime;
		audioSource = GetComponent<AudioSource>();
		audioSource.playOnAwake = playOnAwake ;
		if( audioSource.playOnAwake == true){
			audioSource.clip = sound01;
			audioSource.Play ();
			if(loopCheck == false){
				Destroy(this);
			}
		}else{
			loopTime = 0;
		}

	}

	void Update () {
		loopTime -= Time.deltaTime;
		timer += Time.deltaTime;
		if((timer >= waitTime) && (flag == false) && (loopTime <= 0)){
			//sound01.PlayOneShot(sound01.clip);
			
			audioSource.clip = sound01;
			audioSource.Play ();
			flag = false;
			loopTime = loopWaitTime;
			if( loopCheck == false){
				Destroy(this);
			}

		}
	}
}