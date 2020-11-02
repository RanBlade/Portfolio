using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An enum that will be used to determine if the player is AI or human
public enum PLAYER_TYPE
{
    HUMAN = 1,
    COMPUTER = 2
}
public class Player : MonoBehaviour
{
    [SerializeField]
    private PLAYER_TYPE myplayerType = PLAYER_TYPE.HUMAN;

    public DiagnosticGameManager currentGameManager;
    public PLAYER myTurnPosition;
    // Start is called before the first frame update
    void Start()
    {
        currentGameManager = GameObject.Find("GameManager").GetComponent<DiagnosticGameManager>();

        if (currentGameManager == null)
        {
            Debug.LogError("Error finding Game Manager. URGENT FIX NOW");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(myplayerType == PLAYER_TYPE.COMPUTER && currentGameManager.IsCurrentGameActive())
        {
            if(IsItMyTurn())
            {
                PlayAToken(-1);
            }
        }
    }

    //a function to play your turn. This will only be called if the player is an AI
    public void PlayAToken(int id)
    {
        if (myplayerType == PLAYER_TYPE.HUMAN)
        {
            currentGameManager.enterMove(id);
        }
        else if(myplayerType == PLAYER_TYPE.COMPUTER)
        {
            int playedGridPosition = CheckIfAICanWin();
            if(playedGridPosition != -1)
            {
                currentGameManager.enterMove(playedGridPosition);
                return;
            }
            playedGridPosition = CheckIfAICanBlock();
            if (playedGridPosition != -1)
            {
                currentGameManager.enterMove(playedGridPosition);
                return;
            }
            else
            {
                playedGridPosition = Random.Range(0, 9);
                currentGameManager.enterMove(playedGridPosition);
            }
            
        }
    }

    //A function ot check if it is the current players turn
    public bool IsItMyTurn()
    {
        if (myTurnPosition == currentGameManager.getTurn())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int CheckIfAICanWin()
    {
        //TTT ARRAY ARRANGEMENT
        //[0][1][2]
        //[3][4][5]
        //[6][7][8]

        int[] tempGameGrid = currentGameManager.GetGameState();
        int consecutiveTokenCount = 0;

        Debug.Log("My playing position is: " + myTurnPosition);
        //check if top row can win---------------------------------------------------
        for(int i = 0; i < 3; i++)
        {
            if(tempGameGrid[i] == (int)myTurnPosition)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Win: Top Row Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if(tempGameGrid[0] == 0)
            {
                return 0;
            }
            if (tempGameGrid[1] == 0)
            {
                return 1;
            }
            if (tempGameGrid[2] == 0)
            {
                return 2;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if mid row can win---------------------------------------------------
        for (int i = 3; i < 6; i++)
        {
            if (tempGameGrid[i] == (int)myTurnPosition)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Win: Mid Row Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[3] == 0)
            {
                return 3;
            }
            if (tempGameGrid[4] == 0)
            {
                return 4;
            }
            if (tempGameGrid[5] == 0)
            {
                return 5;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if bot row can win---------------------------------------------------
        for (int i = 6; i < 9; i++)
        {
            if (tempGameGrid[i] == (int)myTurnPosition)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Win: Bot Row Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[6] == 0)
            {
                return 6;
            }
            if (tempGameGrid[7] == 0)
            {
                return 7;
            }
            if (tempGameGrid[8] == 0)
            {
                return 8;
            }
        }
        //--------------------------------------------------------------------------

        //TTT ARRAY ARRANGEMENT
        //[0][1][2]
        //[3][4][5]
        //[6][7][8]

        consecutiveTokenCount = 0;
        //check if left Col can win---------------------------------------------------
        for (int i = 0; i < 7; i+=3)
        {
            if (tempGameGrid[i] == (int)myTurnPosition)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Win: Left Colum Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[0] == 0)
            {
                return 0;
            }
            if (tempGameGrid[3] == 0)
            {
                return 3;
            }
            if (tempGameGrid[6] == 0)
            {
                return 6;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if Mid Col can win---------------------------------------------------
        for (int i = 1; i < 8; i += 3)
        {
            if (tempGameGrid[i] == (int)myTurnPosition)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Win: Mid Colum Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[1] == 0)
            {
                return 1;
            }
            if (tempGameGrid[4] == 0)
            {
                return 4;
            }
            if (tempGameGrid[7] == 0)
            {
                return 7;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if Right Col can win---------------------------------------------------
        for (int i = 2; i < 9; i += 3)
        {
            if (tempGameGrid[i] == (int)myTurnPosition)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Win: Right Colum Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[2] == 0)
            {
                return 2;
            }
            if (tempGameGrid[5] == 0)
            {
                return 5;
            }
            if (tempGameGrid[8] == 0)
            {
                return 8;
            }
        }
        //--------------------------------------------------------------------------

        //TTT ARRAY ARRANGEMENT
        //[0][1][2]
        //[3][4][5]
        //[6][7][8]

        consecutiveTokenCount = 0;
        //check if diag 1 can win---------------------------------------------------
        for (int i = 0; i < 9; i += 4)
        {
            if (tempGameGrid[i] == (int)myTurnPosition)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Win: Diag 1 Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[0] == 0)
            {
                return 0;
            }
            if (tempGameGrid[4] == 0)
            {
                return 4;
            }
            if (tempGameGrid[8] == 0)
            {
                return 8;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if Diag 2 can win---------------------------------------------------
        for (int i = 2; i < 7; i += 2)
        {
            if (tempGameGrid[i] == (int)myTurnPosition)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Win: Diag 2 Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[2] == 0)
            {
                return 2;
            }
            if (tempGameGrid[4] == 0)
            {
                return 4;
            }
            if (tempGameGrid[6] == 0)
            {
                return 6;
            }
        }
        //--------------------------------------------------------------------------

        //-1 gets returned if the AI cannot win the game
        return -1;
    }

    private int CheckIfAICanBlock()
    {
        //TTT ARRAY ARRANGEMENT
        //[0][1][2]
        //[3][4][5]
        //[6][7][8]

        int[] tempGameGrid = currentGameManager.GetGameState();
        int consecutiveTokenCount = 0;

        Debug.Log("My playing position is: " + myTurnPosition);
        //check if top row can win---------------------------------------------------
        for (int i = 0; i < 3; i++)
        {
            if (tempGameGrid[i] != (int)myTurnPosition && tempGameGrid[i] != 0)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Block: Top Row Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[0] == 0)
            {
                return 0;
            }
            if (tempGameGrid[1] == 0)
            {
                return 1;
            }
            if (tempGameGrid[2] == 0)
            {
                return 2;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if mid row can win---------------------------------------------------
        for (int i = 3; i < 6; i++)
        {
            if (tempGameGrid[i] != (int)myTurnPosition && tempGameGrid[i] != 0)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Block: Mid Row Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[3] == 0)
            {
                return 3;
            }
            if (tempGameGrid[4] == 0)
            {
                return 4;
            }
            if (tempGameGrid[5] == 0)
            {
                return 5;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if bot row can win---------------------------------------------------
        for (int i = 6; i < 9; i++)
        {
            if (tempGameGrid[i] != (int)myTurnPosition && tempGameGrid[i] != 0)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Block: Bot Row Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[6] == 0)
            {
                return 6;
            }
            if (tempGameGrid[7] == 0)
            {
                return 7;
            }
            if (tempGameGrid[8] == 0)
            {
                return 8;
            }
        }
        //--------------------------------------------------------------------------

        //TTT ARRAY ARRANGEMENT
        //[0][1][2]
        //[3][4][5]
        //[6][7][8]

        consecutiveTokenCount = 0;
        //check if left Col can win---------------------------------------------------
        for (int i = 0; i < 7; i += 3)
        {
            if (tempGameGrid[i] != (int)myTurnPosition && tempGameGrid[i] != 0)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Block: Left Colum Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[0] == 0)
            {
                return 0;
            }
            if (tempGameGrid[3] == 0)
            {
                return 3;
            }
            if (tempGameGrid[6] == 0)
            {
                return 6;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if Mid Col can win---------------------------------------------------
        for (int i = 1; i < 8; i += 3)
        {
            if (tempGameGrid[i] != (int)myTurnPosition && tempGameGrid[i] != 0)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Block: Mid Colum Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[1] == 0)
            {
                return 1;
            }
            if (tempGameGrid[4] == 0)
            {
                return 4;
            }
            if (tempGameGrid[7] == 0)
            {
                return 7;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if Right Col can win---------------------------------------------------
        for (int i = 2; i < 9; i += 3)
        {
            if (tempGameGrid[i] != (int)myTurnPosition && tempGameGrid[i] != 0)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Block: Right Colum Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[2] == 0)
            {
                return 2;
            }
            if (tempGameGrid[5] == 0)
            {
                return 5;
            }
            if (tempGameGrid[8] == 0)
            {
                return 8;
            }
        }
        //--------------------------------------------------------------------------

        //TTT ARRAY ARRANGEMENT
        //[0][1][2]
        //[3][4][5]
        //[6][7][8]

        consecutiveTokenCount = 0;
        //check if diag 1 can win---------------------------------------------------
        for (int i = 0; i < 9; i += 4)
        {
            if (tempGameGrid[i] != (int)myTurnPosition && tempGameGrid[i] != 0)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Block: Diag 1 Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[0] == 0)
            {
                return 0;
            }
            if (tempGameGrid[4] == 0)
            {
                return 4;
            }
            if (tempGameGrid[8] == 0)
            {
                return 8;
            }
        }
        //--------------------------------------------------------------------------
        consecutiveTokenCount = 0;
        //check if Diag 2 can win---------------------------------------------------
        for (int i = 2; i < 7; i += 2)
        {
            if (tempGameGrid[i] != (int)myTurnPosition && tempGameGrid[i] != 0)
            {
                consecutiveTokenCount++;
            }
        }

        Debug.Log("Block: Diag 2 Check count " + consecutiveTokenCount);

        if (consecutiveTokenCount >= 2)
        {
            if (tempGameGrid[2] == 0)
            {
                return 2;
            }
            if (tempGameGrid[4] == 0)
            {
                return 4;
            }
            if (tempGameGrid[6] == 0)
            {
                return 6;
            }
        }
        //--------------------------------------------------------------------------

        //-1 gets returned if the AI cannot win the game
        return -1;
    }

}
