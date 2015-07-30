using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UtilityScripts;


public class WordLib : MonoBehaviour {

	//private List gameWords = new IList();
	public List<WordLibEntry> gameWords = new List<WordLibEntry>();
	// Use this for initialization
	void Start () {


	

	}
	
	// Update is called once per frame
	void Update () {

	}

	public WordLibEntry GetWord()
	{
		int libMax = (int)gameWords.Count;
		int randomLibEntry = Random.Range(0, libMax);
		return gameWords[randomLibEntry];
	}
}
