using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBox : MonoBehaviour {

	public Text scoreBoxRef;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		scoreBoxRef.text = GetComponentInParent<UIRefrences>().gameStateRefUI.GetScore().ToString();
	}
}
