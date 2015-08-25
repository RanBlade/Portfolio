using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Collections;
using UtilityScripts;

public class UIRefrences : MonoBehaviour
{

    public bool nameNotEntered = false;
    public gameState gameStateRefUI;
    public GameObject letterGuessInputField;
    public GameObject highScoreInputField;

    // Use this for initialization
    void Start()
    {
       // lettGuessInputField = GameObject.Find("LetterGessInput");
        EventSystem.current.SetSelectedGameObject(letterGuessInputField, null);
        EditorGUI.FocusTextInControl("LetterGuessInput");
    }

    // Update is called once per frame
    void Update()
    {
        //Depending on the current game state have specfic input field highlighted.
        if (gameStateRefUI.GetPlayingState() == PLAYINGSTATES.playerTurn)
        {
            EventSystem.current.SetSelectedGameObject(letterGuessInputField, null);
        }
        else if(gameStateRefUI.GetPlayingState() == PLAYINGSTATES.HighScore)
        {
            EventSystem.current.SetSelectedGameObject(highScoreInputField, null);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null, null);
        }

        //If name is not entered into high score field display error message.
        if (!nameNotEntered)
        {
            GameObject.Find("NoNameFlag").GetComponent<CanvasGroup>().alpha = 0;

        }
        else if (nameNotEntered)
        {
            GameObject.Find("NoNameFlag").GetComponent<CanvasGroup>().alpha = 1;

        }

    }

}
