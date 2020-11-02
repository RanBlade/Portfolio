using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{



    public
    // Use this for initialization
    void Start()
    {
        Debug.Log("Buttons loaded");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayerGameButton()
    {
        //if(GameManager.instance.PlayAgain == false)
        //GameManager.instance.PlayAgain = true;
        SceneManager.LoadScene("Town");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
} 
