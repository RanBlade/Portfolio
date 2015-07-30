using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIRefrences : MonoBehaviour {

	public gameState gameStateRefUI;
	private string temp;
	private bool test = true;
	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
		if(!test)
		{
			Text testtext = transform.Find("Title").GetComponent<Text>();
			
			for(int i = 0; i < gameStateRefUI.GetCurrentWordSize(); i++)
			{
				temp+="_";
			}
			Debug.Log(temp + "Temp Length: " + temp.Length);
			testtext.text = temp;

			test=true;
		}
	
	}
}
