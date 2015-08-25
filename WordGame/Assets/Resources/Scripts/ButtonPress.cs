using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ButtonPress : MonoBehaviour
{

    public InputField inputFieldRef;

    private gameState gameStateRef;


    private bool activeElement = true;
    // Use this for initialization
    void Start()
    {
        gameStateRef = GetComponentInParent<UIRefrences>().gameStateRefUI;


        if (gameStateRef == null)
        {
            Debug.Log("GameState is null");
            activeElement = false;
        }
        //	inputFieldRef = GetComponentInParent<InputField>();
        if (inputFieldRef == null)
        {
            Debug.Log("InputField is null");
            activeElement = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Click()
    {
        Debug.Log("Button Clicked");
        if (activeElement)
        {
            if (inputFieldRef.text.Length != 0)
            {
                char tempChar = inputFieldRef.text.ToCharArray()[0];
                gameStateRef.Guess(tempChar);
            }
        }
    }
    public void ClickQuit()
    {
        Debug.Log("Quiting Application");
        gameStateRef.QuitGame();
    }
    public void ClickPlayAgain()
    {
        gameStateRef.PlayAgain(true);
    }
    public void ClickPlayAgainNO()
    {
        gameStateRef.PlayAgain(false);
    }
    public void ClickLeaderBoard()
    {
        gameStateRef.ShowLeaderBoard();
    }
    public void ClickCloseLeaderBoard()
    {
        gameStateRef.CloseLeaderBoard();
    }
    public void ClickNewGame()
    {
        gameStateRef.PlayerNewGame();
    }
    public void ClickHighScoreNameDone()
    {
        if (inputFieldRef.text == "")
        {
            Debug.Log("You did not enter a name");
            GetComponentInParent<UIRefrences>().nameNotEntered = true;
        }
        else
        {
            gameStateRef.EnterName(inputFieldRef.text);
            GetComponentInParent<UIRefrences>().nameNotEntered = false;

        }
    }
}
