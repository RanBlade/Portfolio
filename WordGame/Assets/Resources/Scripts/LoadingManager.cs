using UnityEngine;
using System.Collections;

public class LoadingManager : MonoBehaviour {

	public int LoadTime;
	public string SceneName;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Invoke("Load" , LoadTime);
	}

	void Load()
	{
		Application.LoadLevel(SceneName);
	}
}
