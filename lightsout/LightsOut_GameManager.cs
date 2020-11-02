using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TEST_MODE
{
    TEST_WIN,
    PLAY_GAME
}
public class LightsOut_GameManager : MonoBehaviour
{
    [Header("State Data")]
    [SerializeField]
    private ToggleLight[] lightArray;
    [SerializeField]
    private int turnCount = 0;
    [SerializeField]
    private int MaxTurns = 0;
    [SerializeField]
    private bool isGameActive = true;

    [Header("Game Settings")]
    [SerializeField]
    private int RandomLightsOnMAX = 0;
    private int CurrentRandomLightsOn = 0;

    [Header("Debug and TestMode Settings")]
    public TEST_MODE currentGameMode = TEST_MODE.PLAY_GAME;
    public int testID;
    public bool leftEdge = false;
    public bool rightEdge = false;
    public bool TopEdge = false;
    public bool BottomEdge = false;

    [Header("UI Settings and objects")]
    public Text winLoseText;
    public Text turnCountText;
    public Text turnCountMaxText;
    // Start is called before the first frame update
    void Start()
    {
        lightArray = new ToggleLight[25];
        GameObject[] tempArry = GameObject.FindGameObjectsWithTag("GameLight");
        int i = 0;
        foreach(GameObject go in tempArry)
        {
            lightArray[i] = go.GetComponent<ToggleLight>();
            i++;
        }

        UpdateTurnCountText();
        RandomizeGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGridLoc(int loc)
    {
        lightArray[loc].Toggle();

        int[] tempArray = lightArray[loc].GetAdjacentLights();

        for(int i = 0; i < 4; i++)
        {
            if(tempArray[i] != -1)
            {
                lightArray[tempArray[i]].Toggle();
            }
        }

        bool checkWin = CheckForWin();

        turnCount++;
        UpdateTurnCountText();

        if(checkWin)
        {
            Debug.Log("PLAYER HAS WON THE GAME");
            isGameActive = false;
            winLoseText.text = "YOU HAVE WON THE GAME!!!";
            winLoseText.gameObject.SetActive(true);
        }
        else if(!checkWin && turnCount < MaxTurns)
        {
            Debug.Log("Lights are still on!");
        }
        else if(!checkWin && turnCount >= MaxTurns)
        {
            Debug.Log("You Lose!");
            isGameActive = false;
            winLoseText.text = "YOU HAVE LOST THE GAME!";
            winLoseText.gameObject.SetActive(true);
        }
    }

    public void RandomizeGame()
    {
        if (currentGameMode == TEST_MODE.PLAY_GAME)
        {
            CurrentRandomLightsOn = 0;

            foreach (ToggleLight tl in lightArray)
            {
                int rand = Random.Range(1, 3);

                if (CurrentRandomLightsOn >= RandomLightsOnMAX)
                {
                    rand = 2;
                }

                Debug.Log("Light " + tl.gridID + " value is: " + rand);

                if (rand == 1)//light is on
                {
                    tl.SetLightStatus(true);
                    CurrentRandomLightsOn++;
                }
                else if (rand == 2)//Light is off
                {
                    tl.SetLightStatus(false);
                }

            }
        }
        else if (currentGameMode == TEST_MODE.TEST_WIN)
        {
            //If testing an edge grid location make sure you comment out the appropriate grid modifiers
            //ie if you are testing gridID 0 then you would comment out the -1 and the -5 lines 

            foreach( ToggleLight tl in lightArray)
            {
                tl.SetLightStatus(false);
            }

            if (testID <= 4)
                TopEdge = true;
            if (testID == 0 || testID == 5 || testID == 10 || testID == 15 || testID == 20)
                leftEdge = true;
            if (testID >= 20 && testID <= 24)
                BottomEdge = true;
            if (testID == 4 || testID == 9 || testID == 14 || testID == 19 || testID == 24)
                rightEdge = true;


            
            if (TopEdge && leftEdge)
            {
                lightArray[testID].SetLightStatus(true);
                lightArray[testID + 1].SetLightStatus(true);
                lightArray[testID + 5].SetLightStatus(true);
            }
            
            else if(TopEdge && rightEdge)
            {
                lightArray[testID].SetLightStatus(true);
                lightArray[testID - 1].SetLightStatus(true);
                lightArray[testID + 5].SetLightStatus(true);
            }
           
            else if(leftEdge && BottomEdge)
            {
                lightArray[testID].SetLightStatus(true);
                lightArray[testID + 1].SetLightStatus(true);
                lightArray[testID - 5].SetLightStatus(true);
            }

            else if(rightEdge && BottomEdge)
            {
                lightArray[testID].SetLightStatus(true);
                lightArray[testID - 1].SetLightStatus(true);
                lightArray[testID - 5].SetLightStatus(true);

            }

            else if(BottomEdge)
            {
                lightArray[testID].SetLightStatus(true);
                lightArray[testID - 1].SetLightStatus(true);
                lightArray[testID + 1].SetLightStatus(true);
                lightArray[testID - 5].SetLightStatus(true);
            }

            else if (TopEdge)
            {
                lightArray[testID].SetLightStatus(true);
                lightArray[testID - 1].SetLightStatus(true);
                lightArray[testID + 1].SetLightStatus(true);
                lightArray[testID + 5].SetLightStatus(true);

            }

            else if (rightEdge)
            {
                lightArray[testID].SetLightStatus(true);
                lightArray[testID - 1].SetLightStatus(true);
                lightArray[testID -5].SetLightStatus(true);
                lightArray[testID + 5].SetLightStatus(true);

            }

            else if (leftEdge)
            {
                lightArray[testID].SetLightStatus(true);
                lightArray[testID - 5].SetLightStatus(true);
                lightArray[testID + 1].SetLightStatus(true);
                lightArray[testID + 5].SetLightStatus(true);

            }

            else if( !BottomEdge && !TopEdge && !leftEdge && !rightEdge)
            {
                lightArray[testID].SetLightStatus(true);
                lightArray[testID - 1].SetLightStatus(true);
                lightArray[testID + 1].SetLightStatus(true);
                lightArray[testID + 5].SetLightStatus(true);
                lightArray[testID - 5].SetLightStatus(true);
            }
        }
    }

    private bool CheckForWin()
    {
        foreach(ToggleLight tl in lightArray)
        {
            if(tl.GetLightStatus())
            {
                return false;
            }
        }

        return true; //if we got here then all lights should be off and the game is over.
    }

    public bool CheckIfGameIsActive()
    {
        return isGameActive;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("RanaldiEric_LO");
    }

    private void UpdateTurnCountText()
    {
        turnCountText.text = turnCount.ToString();
        turnCountMaxText.text = MaxTurns.ToString();
    }
}
