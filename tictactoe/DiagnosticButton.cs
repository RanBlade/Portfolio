using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagnosticButton : MonoBehaviour
{

    //TTT ARRAY ARRANGEMENT
    //[0][1][2]
    //[3][4][5]
    //[6][7][8]

    ///UNCOMMENT between ========== and fill out information accordingly
    /// <summary>
    /// GLOBAL SCOPE CLASS MEMBER VARIABLES
    //	Project Name: Tic-Tac-Toe
    //	Date:	1-13-2020
    //	Links:	NA
    /// </summary>
    //==========


    //Game piece variables to test for game piece placement
    public bool gamePieceHasbeenPlaced = false;
    public int gameBoardID;

    public DiagnosticGameManager currentGameManager;
	//==========

    public void Start()
    {
        currentGameManager = GameObject.Find("GameManager").GetComponent<DiagnosticGameManager>();

        if(currentGameManager == null)
        {
            Debug.LogError("Error finding Game Manager. URGENT FIX NOW");
        }
    }

	private void OnMouseUp()
	{
        
		if(gamePieceHasbeenPlaced == false && currentGameManager.IsCurrentGameActive())
		{
            GameObject currentPlayer = currentGameManager.GetCurrentPlayer();
            currentPlayer.GetComponent<Player>().PlayAToken(gameBoardID);
            //PlaceGameToken(gameBoardID);
            gamePieceHasbeenPlaced = true;
		}
        else if(!currentGameManager.IsCurrentGameActive())
        {
            Debug.Log("Current Game is not playabkle");
        }
        else if(gamePieceHasbeenPlaced == true && currentGameManager.IsCurrentGameActive())
        {
            Debug.Log("Location has already been played, choose another location");
        }
        
	}

	public void PlaceGameToken(PLAYER assignedPlayer)
	{

        if (assignedPlayer >= 0 /*&& haveIBeenClicked == false*/)
        {                   
            if (assignedPlayer == PLAYER.PLAYER_ONE)
            {
                
                gameObject.transform.Find("PlayerOneToken").gameObject.SetActive(true);
            }
            else if (assignedPlayer == PLAYER.PLAYER_TWO)
            {
                gameObject.transform.Find("PlayerTwoToken").gameObject.SetActive(true);
            }
            
            //Turn off the unselected visual
            gameObject.GetComponent<MeshRenderer>().enabled = false;

            
        }
	}

    public void ResetButtonState()
    {
        gamePieceHasbeenPlaced = false;
        gameObject.transform.Find("PlayerOneToken").gameObject.SetActive(false);
        gameObject.transform.Find("PlayerTwoToken").gameObject.SetActive(false);

        
    }

    public int GetID()
    {
        return gameBoardID;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
