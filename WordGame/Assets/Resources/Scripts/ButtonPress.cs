using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour {

	public InputField inputFieldRef;

	private gameState gameStateRef;

	private bool activeElement = true;
	// Use this for initialization
	void Start () {
		gameStateRef  = GetComponentInParent<UIRefrences>().gameStateRefUI;

		if(gameStateRef == null){
			Debug.Log ("GameState is null");
			activeElement = false;
		}
	//	inputFieldRef = GetComponentInParent<InputField>();
		if(inputFieldRef == null)
		{
			Debug.Log("InputField is null");
			activeElement = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Click()
	{
		if(activeElement)
		{
			if(inputFieldRef.text.Length != 0)
			{
				char tempChar = inputFieldRef.text.ToCharArray()[0];
				gameStateRef.Guess(tempChar);
			}
		}
	}
}
