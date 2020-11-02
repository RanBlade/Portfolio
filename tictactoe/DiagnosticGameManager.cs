using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PLAYER
{
    PLAYER_ONE = 1,
    PLAYER_TWO = 2
}
public class DiagnosticGameManager : MonoBehaviour
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
    /// 

    //==========
    //All game manager data is private to ensure data integrity.
    private const int GAME_GRID_SIZE = 9;
    private const int MAX_TURNS = 9;

    [SerializeField]
    private int[] gameBoardArray;
    [SerializeField]
    private PLAYER currentPlayerTurn = PLAYER.PLAYER_ONE;

    private int winnerOfTheGame = 0;
    private bool bisCurrentGameActive = true;
    private int turnCount = 0;

    [SerializeField]
    private GameObject[] gameBoardGOArray;

    //UI control variables
    public GameObject playerTunUIObj;
    public GameObject playerWinnerUIObj;

    //Player refrences
    public GameObject PlayerOneRef;
    public GameObject PlayerTwoRef;
	//==========


	//This function should be called each time a player enters a move
	public bool checkWinFunction()
	{
        for(int player = 1; player <= 2; player++)
        {
            //across wins
            if (gameBoardArray[0] == player && gameBoardArray[1] == player && gameBoardArray[2] == player)
            {
                Debug.Log("Top Row Win");
                winnerOfTheGame = player;
                
                return true;
            }
            else if (gameBoardArray[3] == player && gameBoardArray[4] == player && gameBoardArray[5] == player)
            {
                Debug.Log("Mid Row Win");
                winnerOfTheGame = player;
               
                return true;
            }
            else if (gameBoardArray[6] == player && gameBoardArray[7] == player && gameBoardArray[8] == player)
            {
                Debug.Log("Bottom Row Win");
                winnerOfTheGame = player;
               
                return true;
            }
            //vertical wins
            else if (gameBoardArray[0] == player && gameBoardArray[3] == player && gameBoardArray[6] == player)
            {
                Debug.Log("Column 1 Win");
                winnerOfTheGame = player;
               
                return true;
            }
            else if (gameBoardArray[1] == player && gameBoardArray[4] == player && gameBoardArray[7] == player)
            {
                Debug.Log("Column 2 Win");
                winnerOfTheGame = player;
               
                return true;
            }
            else if (gameBoardArray[2] == player && gameBoardArray[5] == player && gameBoardArray[8] == player)
            {
                Debug.Log("Column 3 Win");
                winnerOfTheGame = player;
                
                return true;
            }
            //diagnoal wins
            else if (gameBoardArray[0] == player && gameBoardArray[4] == player && gameBoardArray[8] == player)
            {
                Debug.Log("Diagonal 1 Win");
                winnerOfTheGame = player;
            
                return true;
            }
            else if (gameBoardArray[2] == player && gameBoardArray[4] == player && gameBoardArray[6] == player)
            {
                Debug.Log("Diagonal 2 Win");
                winnerOfTheGame = player;
             
                return true;
            }
        }

		return false;
	}

	// Start is called before the first frame update
	void Start()
    {
        playerTunUIObj.GetComponent<Text>().text = "Player One";

        PlayerOneRef.GetComponent<Player>().myTurnPosition = PLAYER.PLAYER_ONE;
        PlayerTwoRef.GetComponent<Player>().myTurnPosition = PLAYER.PLAYER_TWO;
    }

	public void enterMove(int id)
	{
        Debug.Log("Enter Move id: " + id);
		if (IsCurrentGameActive() && gameBoardArray[id] == 0)
		{
            //gridArray[_index] == variableType_value;
            GameObject gameBoardCell = GetPlayedGameCell(id);

            if(!gameBoardCell)
            {
                Debug.Log("Error finding gameboard cell. FIX NOW");
                return;
            }
            gameBoardCell.GetComponent<DiagnosticButton>().PlaceGameToken(currentPlayerTurn);
            gameBoardArray[id] = (int)currentPlayerTurn;
            turnCount++;
            //Definetly better ways to switch player turns but this is fairly readable.
            switch (currentPlayerTurn)
            {
                case PLAYER.PLAYER_ONE:
                    {
                        currentPlayerTurn = PLAYER.PLAYER_TWO;
                        playerTunUIObj.GetComponent<Text>().text = "Player Two";
                        break;
                    }
                case PLAYER.PLAYER_TWO:
                    {
                        currentPlayerTurn = PLAYER.PLAYER_ONE;
                        playerTunUIObj.GetComponent<Text>().text = "Player One";
                        break;
                    }
            }
		}

	    bool checkIfPlayerWon = checkWinFunction();
        Debug.Log("Turn Count: " + turnCount);
        if (checkIfPlayerWon == true /*&& turnCounter >= ????? */)
        {
            Debug.Log("Player " + winnerOfTheGame + " has won the game");
            string winnerString = "Player " + winnerOfTheGame + " has won the game!";
            playerWinnerUIObj.GetComponent<Text>().text = winnerString;
            playerWinnerUIObj.GetComponent<Text>().color = Color.green;
            playerWinnerUIObj.SetActive(true);
            bisCurrentGameActive = false;
  
        }

        else if (turnCount >= MAX_TURNS)
        {
            Debug.Log("Game ends in a draw");
            string winnerString = "Game has ended in a draw";
            playerWinnerUIObj.GetComponent<Text>().text = winnerString;
            playerWinnerUIObj.GetComponent<Text>().color = Color.gray;
            playerWinnerUIObj.SetActive(true);
            bisCurrentGameActive = false;

        }

		//SET TURN TO THE OTHER PLAYER'S TURNTYPE

	}

    public bool IsCurrentGameActive()
    {
        return bisCurrentGameActive;
    }

	//remove void and REPLACE VARIABLETYPE with correct datatype from currentTurn above
	public PLAYER getTurn()
	{

		return currentPlayerTurn;
	}

    public GameObject GetCurrentPlayer()
    {
        if(getTurn() == PLAYER.PLAYER_TWO)
        {
            return PlayerTwoRef;
        }
        else if(getTurn() == PLAYER.PLAYER_ONE)
        {
            return PlayerOneRef;
        }
        return null;
    }
    public void ResetGame()
    {
        gameBoardArray = new int[GAME_GRID_SIZE];
        bisCurrentGameActive = true;

        turnCount = 0;
        winnerOfTheGame = 0;
        currentPlayerTurn = PLAYER.PLAYER_ONE;
        playerTunUIObj.GetComponent<Text>().text = "Player One";
        playerWinnerUIObj.SetActive(false);

        Debug.Log(gameBoardGOArray.Length);
        for(int i = 0; i < gameBoardGOArray.Length; i++)
        {
            gameBoardGOArray[i].GetComponent<DiagnosticButton>().ResetButtonState();            
        }
    }

    private GameObject GetPlayedGameCell(int id)
    {
        for(int i = 0; i < gameBoardGOArray.Length; i++)
        {
            if(gameBoardGOArray[i].GetComponent<DiagnosticButton>().GetID() == id)
            {
                return gameBoardGOArray[i];
            }
        }

        return null;
    }

    public int[] GetGameState()
    {
        return gameBoardArray;
    }
	// Update is called once per frame
	void Update()
    {
        
    }
}
