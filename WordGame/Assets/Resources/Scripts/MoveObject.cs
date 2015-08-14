using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {


	public Vector3 lerpEnd;
	public int Delay;
	private int totalSeconds = 0;
	private float timer = 1f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		TimerUpdate();
		if(totalSeconds == Delay)
			transform.position = Vector3.Lerp(transform.position, lerpEnd, Time.deltaTime * 0.1f);
	}

	void TimerUpdate()
	{
		Debug.Log ("total seconds: " + totalSeconds + " Delay: " + Delay + " Timer: " + timer);
		if(totalSeconds < Delay)
		{
			if(timer > 0)
				timer -= Time.deltaTime;
			else if(timer <= 0)
			{
				totalSeconds++;
				timer = 1.0f;
			}
		}
	}
}
