using UnityEngine;
using System.Collections;

public class DestroyObj : MonoBehaviour {
	
	public float timer = 0.5f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0){
			Object.Destroy(gameObject);
		}
	}
}
