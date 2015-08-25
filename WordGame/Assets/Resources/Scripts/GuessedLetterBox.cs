using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuessedLetterBox : MonoBehaviour {

	public Text guessedLettersRef;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        guessedLettersRef.text = GetComponentInParent<UIRefrences>().gameStateRefUI.GetGuessedLetters();
	}
}
