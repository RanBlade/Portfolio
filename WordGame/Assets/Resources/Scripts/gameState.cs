using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using UtilityScripts;

public class gameState : MonoBehaviour
{

    //game timer settings
    public int GameLength;
    private int totalSeconds = 0;
    private float timer = 1f;

    //score modifier constants
    const int RIGHTGUESS = 15;
    const int WRONGGUESS = -5;
    const int WIN = 100;

    //game counters
    //private int roundCount, guessCount;
    private int wrongGuessCount, gameScore;
    private const int highScoreMAX = 10;

    //game true/false conditions
    private bool playerGuessed;
    //private bool pickWord;
    private bool enterState;
    private PLAYAGAINFLAG playAgain;

    // word variables for the game state
    private char guessedLetter;
    private List<char> guessedLetters;
    private string formattedGuessedLetters;
    private string formmatedHighScore;
    private string hiddenWord;
    private WordLibEntry currentWord;

    //stored list of the gameblocks for the image
    private List<GameObject> gameImageBlocks;
    private Texture gameImage;

    //current States
    private MAINSTATES currentMstate;
    private PLAYINGSTATES currentGstate;
    private NEWGAMETYPE gameType;

    //scpre board list
    public List<LeaderBoardEntry> highScores;
    bool highScoreNameEntered;
    string highScoreName;

    // Use this for initialization
    void Start()
    {
        //fix this after testing to proper start states
        currentMstate = MAINSTATES.menu;
        currentGstate = PLAYINGSTATES.playerTurn;

        //create memory for the gamestate containeers
        gameImageBlocks = new List<GameObject>();
        guessedLetters = new List<char>();
        currentWord = new WordLibEntry();
        highScores = new List<LeaderBoardEntry>();

        StartCoroutine(LoadHighScores());

        CreateGameImage();
        LoadGameImage();
        SetGameImageActive(false);

        enterState = false;
        playAgain = PLAYAGAINFLAG.notSelected;
        gameType = NEWGAMETYPE.newGame;

        SwitchMainState(MAINSTATES.menu);
        //SwitchPlayingState(PLAYINGSTATES.newGame);

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentMstate)
        {

            case MAINSTATES.menu:
                {
                    if (enterState)
                    {
                        HideGameUI();
                        SetGameImageActive(false);
                        //FormatHighScore();
                        enterState = false;
                    }
                    break;
                }
            case MAINSTATES.loading:
                {
                    break;
                }
            case MAINSTATES.playing:
                {
                    switch (currentGstate)
                    {
                        case PLAYINGSTATES.newGame:
                            {
                                if (enterState)
                                {
                                    StartNewGame();
                                    enterState = false;
                                    SwitchPlayingState(PLAYINGSTATES.playerTurn);
                                }

                                break;
                            }
                        case PLAYINGSTATES.playerTurn:
                            {
                                if (enterState)
                                {
                                    enterState = false;
                                }
                                if (playerGuessed)
                                {
                                    ProcessGuess();
                                    ConvertGuessedLettersToString();
                                    CheckWinLose();
                                    playerGuessed = false;
                                }
                                TimerUpdate();
                                if (totalSeconds == GameLength)
                                {
                                    //switch to gameOverState
                                    Debug.Log("Game time has expired!");

                                }
                                
                                break;
                            }
                        case PLAYINGSTATES.mainMenu:
                            {
                                break;
                            }
                        case PLAYINGSTATES.win:
                            {
                                if (enterState)
                                {
                                    //GameObject.Find ("Win").GetComponent<CanvasGroup>().alpha = 1;
                                    ToggleWinUI();
                                    ShowHiddenWord();
                                    Debug.Log("YOU WIN!!! CONGRATS!");
                                    enterState = false;
                                }
                                if (playAgain == PLAYAGAINFLAG.yes)
                                {
                                    SwitchPlayingState(PLAYINGSTATES.newGame);
                                    gameType = NEWGAMETYPE.winGame;
                                    ToggleWinUI();
                                    playAgain = PLAYAGAINFLAG.notSelected;
                                }
                                else if (playAgain == PLAYAGAINFLAG.no)
                                {
                                    if (CheckHighScore(gameScore))
                                    {
                                        ToggleWinUI();
                                        SwitchPlayingState(PLAYINGSTATES.HighScore);
                                    }
                                    else
                                    {
                                        ToggleWinUI();
                                        SwitchMainState(MAINSTATES.menu);
                                    }
                                    playAgain = PLAYAGAINFLAG.notSelected;
                                }
                                break;
                            }
                        case PLAYINGSTATES.loss:
                            {
                                if (enterState)
                                {
                                    ToggleLossUI();
                                    ShowHiddenWord();
                                    Debug.Log("You Lose! Sorry try again");
                                    enterState = false;
                                }
                                if (playAgain == PLAYAGAINFLAG.yes)
                                {
                                    SwitchPlayingState(PLAYINGSTATES.newGame);
                                    gameType = NEWGAMETYPE.LoseGame;
                                    ToggleLossUI();
                                    playAgain = PLAYAGAINFLAG.notSelected;
                                }
                                else if (playAgain == PLAYAGAINFLAG.no)
                                {
                                    if (CheckHighScore(gameScore))
                                    {
                                        ToggleLossUI();
                                        SwitchPlayingState(PLAYINGSTATES.HighScore);
                                    }
                                    else
                                    {
                                        ToggleLossUI();
                                        SwitchMainState(MAINSTATES.menu);
                                    }
                                    playAgain = PLAYAGAINFLAG.notSelected;
                                }
                                break;
                            }
                        case PLAYINGSTATES.HighScore:
                            {

                                if (enterState)
                                {
                                    Debug.Log("ENTER YOUR HIGH SCORE YOU BADASS!");
                                    ToggleEnterNameUI();
                                    highScoreNameEntered = false;
                                    highScoreName = null;
                                    enterState = false;

                                }
                                if (highScoreNameEntered)
                                {
                                    ToggleEnterNameUI();
                                    UpdateHighScoreList(highScoreName, gameScore);
                                    FormatHighScore();
                                    StartCoroutine(SaveHighScores());
                                    SwitchMainState(MAINSTATES.menu);
                                }
                                break;
                            }
                    }
                    break;
                }
            case MAINSTATES.pause:
                {
                    break;
                }
            case MAINSTATES.quit:
                {
                    break;
                }
        }


    }

    void UpdateUI()
    {
    }

    //============================================================================================================================================================================
    //---------------------------------Public accessor fucntions for other objects like UI to interface with the game state-------------------------------------------------------
    //============================================================================================================================================================================

    public string GetCurrentWord()
    {
        return currentWord.word;
    }

    public int GetCurrentWordSize()
    {
        return currentWord.GetWordSize();
    }

    public string GetGuessedLetters()
    {
        return formattedGuessedLetters;
    }

    public string GetHiddenWord()
    {
        return hiddenWord;
    }

    public PLAYINGSTATES GetPlayingState()
    {
        return currentGstate;
    }

    public int GetScore()
    {
        return gameScore;
    }

    public void ShowLeaderBoard()
    {
        ToggleLeaderBoardUI();
    }

    public void CloseLeaderBoard()
    {
        ToggleLeaderBoardUI();
    }

    public string GetFormattedLeaderBoard()
    {
        return formmatedHighScore;
    }

    public void Guess(char tempText)
    {
        Debug.Log("Testing Click and Guess: " + tempText + "!!!");
        guessedLetter = tempText;
        playerGuessed = true;

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayAgain(bool shallWePlayAgain)
    {
        if (shallWePlayAgain)
            playAgain = PLAYAGAINFLAG.yes;
        else if (!shallWePlayAgain)
            playAgain = PLAYAGAINFLAG.no;
        else
            playAgain = PLAYAGAINFLAG.notSelected;
    }

    public void PlayerNewGame()
    {
        SwitchMainState(MAINSTATES.playing);
        SwitchPlayingState(PLAYINGSTATES.newGame);
        gameType = NEWGAMETYPE.newGame;
    }

    public void EnterName(string name)
    {
        Debug.Log("Name entered: " + name);
        highScoreName = name;
        highScoreNameEntered = true;
    }


    //======================================================================================================================================================================================
    //-----------------------------------------------------------------------------Private Functions for the gameState----------------------------------------------------------------------
    //======================================================================================================================================================================================

    /*
	 * Function: ProcessGuess
	 * Purpose: To evalute the guess and decide what to do
	 * 
	 * Argunemnts: NONE
	 * 
	 * Description: Function will check if the guess is a proper guess and if the letter has been guessed.
	 * 				if the letter is a proper letter and has not been guessed. Then it checks the letter against
	 * 				the word. And then it will process Right guess or wrong guess.
	 */
    private void ProcessGuess()
    {
        if (!CheckifLetterUsed(guessedLetter) && char.IsLetter(guessedLetter))
        {
            if (CheckGuess(guessedLetter))
                RightGuess(guessedLetter);
            else
                WrongGuess(guessedLetter);
        }
        else
        {
            //Add logic to handle letting the user know they have already guessed this letter OR they made a wrong selection. 
        }

        guessedLetter = '0';
    }

    /*Function: CheckifLetterUsed
	 * Purpose: To check if the letter has been guessed before.
	 * 
	 * Arguments: char templetter
	 * 
	 * Description: check the passed char from user input against the guessed letters
	 * 				if it has been guessed return true
	 * 				if it has not been guessed return false
	 */
    private bool CheckifLetterUsed(char tempLetter)
    {
        foreach (char c in guessedLetters)
        {
            if (c == tempLetter)
                return true;
        }
        return false;
    }

    /*Function: ConvertGuessedLettersToString
	 * Purpose: To format the guessed letters into a comma seperated list to display in UI
	 * 
	 * arguments: none
	 * 
	 * Description: loop the the array of guessedletter and amke a string of comma seperated letters.
	 */
    private void ConvertGuessedLettersToString()
    {
        formattedGuessedLetters = null;
        foreach (char c in guessedLetters)
        {
            formattedGuessedLetters += (c + ", ");
        }
    }

    /*Function: CheckGuess
	 * Purpose: To check the letter guess against the game word
	 * 
	 * Arguments: 
	 * 			char guessLetter - the letter the user inputerd to the game as a guess
	 * 
	 * Descriptions: It loops through the letters of the word and if it matches returns a yes
	 * 					if it doens't match a letter from the word then it returns false
	 */
    private bool CheckGuess(char guessLetter)
    {
        foreach (char c in currentWord.word)
        {
            if (guessLetter == c || guessLetter == char.ToLower(c))
            {
                Debug.Log("YOU GOT A LETTER!!!");
                return true;
            }
        }
        Debug.Log("Wrong Guess");
        return false;
    }

    /*Function: RightGuess
	 * Purpose: To handle the logic if the player makes a correct guess
	 * 
	 * Arguments: correctGuess
	 * 
	 * Description: Show the hidden letter of the correctGuess. Add the letter to the guessed letter string
	 * 				and increment the score
	 */
    private void RightGuess(char correctGuess)
    {
        guessedLetters.Add(correctGuess);
        ShowHiddenLetter(correctGuess);
        ChangeScore(RIGHTGUESS);
    }

    /*Function: WrongGuess
	 * Purpose: To handle the logic if hte player makes wrong guess
	 * 
	 * Arguments: inccorectGuess
	 * 
	 * Description: Add the incorrectGuess to the guessed letters string. Modify the score
	 * 				and make a game image clue appear
	 */
    private void WrongGuess(char incorrectGuess)
    {
        ShowGameImageOnWrongGuess();
        guessedLetters.Add(incorrectGuess);
        ChangeScore(WRONGGUESS);

    }

    /*Function: ChangeScore
	 * Purpose: To increment the score
	 * 
	 * Arguments: scoreModifer
	 * 
	 * Description: Increment the game score by the provided value
	 */
    private void ChangeScore(int scoreModifer)
    {
        gameScore += scoreModifer;
    }

    /*Function: SetHiddenWord
	 * Purpose: To create a hidden word string to show the player on the game screen
	 * 
	 * Arguments: none
	 * 
	 * Description: iterate through the currentWord and create a string of "_" to match the length of the word
	 */
    private void SetHiddenWord()
    {
        hiddenWord = null;
        foreach (char c in currentWord.word)
        {
            hiddenWord += "_";
        }
    }

    /*Function: ShowHiddenLetter
	 * Purpose: To show the letter on the hidden word when a correct guess happens
	 * 
	 * Arguments: guessedLetter
	 * 
	 * Description: Iterate through the game word. Once the match is found replace the hidden letter with the
	 * 				guessed letter
	 */
    private void ShowHiddenLetter(char guessedLetter)
    {
        int stringIndex = 0;
        char[] tempArray = hiddenWord.ToCharArray();
        foreach (char c in currentWord.word)
        {
            Debug.Log("c: " + c + " guessedLetter: " + guessedLetter);
            if (c == guessedLetter || guessedLetter == char.ToLower(c))
            {
                Debug.Log("Found match " + c);
                tempArray[stringIndex] = c;
            }
            stringIndex++;
        }
        hiddenWord = new string(tempArray);
    }

    /*Function: ShowHiddenWord
     *Purpose: To show the hidden word on loss
     *
     *Arguments: none
     *
     *Description: Set the hidden word to the game word
     */
    private void ShowHiddenWord()
    {
        hiddenWord = currentWord.word;
    }

    /*Function: StartNewGame
	 * Purpose: To do setup for a new game
	 * 
	 * arguments: none
	 * 
	 * Description: Setup the game to be played by first selecting a word to use.
	 * 				then populating the hidden word
	 * 				then loading the currentword image to the image blocks.
	 * 				Set the score and the state to playing...
	 */
    private void StartNewGame()
    {
        //pickWord = true;
        playerGuessed = false;
        //guessCount = 0;
        wrongGuessCount = 0;
        //roundCount = 0;
        if (gameType == NEWGAMETYPE.newGame || gameType == NEWGAMETYPE.LoseGame)
            gameScore = 0;

        currentWord = GetComponent<WordLib>().GetWord();
        SetHiddenWord();

        if (guessedLetters.Count > 0)
            guessedLetters.Clear();
        //if(formattedGuessedLetters.Length > 0)
        formattedGuessedLetters = " ";

        Debug.Log("CurrentWord: " + currentWord.word + " Size: " + currentWord.GetWordSize());

        //Setup functions to setup initial gamestate or do testing
        SetGameImage();
        SetGameImageActive(false);
        BrightenGameUI();

    }

    /*Function: SwitchPlayingState
	 * Purpose: To switch between states
	 * 
	 * arguments: tempState
	 * 
	 * Description: Take state argument and assign to current playing State
	 */
    private void SwitchPlayingState(PLAYINGSTATES tempState)
    {
        currentGstate = tempState;
        enterState = true;
    }

    /*Function: SwitchMainState
	 * Purpose: To switch between states
	 * 
	 * arguments: tempState
	 * 
	 * Description: Take state argument and assign to current playing State
	 */
    private void SwitchMainState(MAINSTATES tempState)
    {
        currentMstate = tempState;
        enterState = true;
    }

    //This function creates the blocks that the game image will be applied to and that the game will use to give hints to the player 
    //if they get wrong guesses.
    /*Function: CreateGameImage
	 * Purpose: To create the game objects that will be used to draw the word images on.
	 * 
	 * Arguments: none
	 * 
	 * Description: Create four game objects. Go through each game object and instatinate a cube and give it a name and parent
	 * 				Then add each created gameObject to the gameImageBlocks list
	 */
    private void CreateGameImage()
    {
        //game blocks that have the images on them to play the game
        GameObject topLeft;
        GameObject topRight;
        GameObject bottomLeft;
        GameObject bottomRight;

        topLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topLeft.name = "TLImage";
        topLeft.transform.SetParent(transform);

        topRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topRight.name = "TRImage";
        topRight.transform.SetParent(transform);

        bottomRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bottomRight.name = "BRImage";
        bottomRight.transform.SetParent(transform);

        bottomLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bottomLeft.name = "BLImage";
        bottomLeft.transform.SetParent(transform);


        gameImageBlocks.Add(topLeft);
        gameImageBlocks.Add(topRight);
        gameImageBlocks.Add(bottomLeft);
        gameImageBlocks.Add(bottomRight);

    }

    //This places the game blocks in the right position on the game screen. Should only have ot be called once.
    /*Function: LoadGameImage
	 * Purpose: To position the gameImageBlocks in the scene
	 * 
	 * Arguments: none
	 * 
	 * Description: Iterate through the gameImageBlocks and place them in the scene aligned next to each other
	 * 				in a 2x2 grid
	 */
    private void LoadGameImage()
    {
        //inital x,y' of image;
        int xPos = 0;
        int yPos = 0;
        int zPos = 300;
        int incrementVal = 25;
        int incrementScale = 25;
        int zRot = 180; //Im not a art guy I don't know why but the iamge has to be rotated 180 on the z axis to be right side up --maybe ill fix later

        foreach (GameObject a in gameImageBlocks)
        {
            Vector3 tempPos = new Vector3(xPos, yPos, zPos);

            a.transform.position = tempPos;
            a.transform.Rotate(0, 0, zRot);
            a.transform.localScale += new Vector3(incrementScale, incrementScale, 1);

            xPos += incrementVal;
            if (xPos >= 50)
            {
                xPos = 0;
                yPos = -25;
            }
        }
    }

    //Loads the texture onto the game blocks
    /*Function: SetGameImage
	 * Purpose: To apply the texture to the gameImageBlocks.
	 * 
	 * Arguments: none
	 * 
	 * Description: Set the gameImage to the currentWord image. Then iterate through each gameImageBlock 
	 * 				and apply a part of the texture to the block. 
	 */
    private void SetGameImage()
    {
        gameImage = currentWord.image;

        float offSetX = 0.0f;
        float offSetY = 0.5f;
        float offSetChange = 0.5f;

        if (gameImage == null)
            Debug.Log("Game Image is NULL");

        foreach (GameObject block in gameImageBlocks)
        {
            block.GetComponent<MeshRenderer>().material.mainTexture = gameImage;
            block.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(0.5f, 0.5f));
            block.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(offSetX, offSetY));

            if (offSetX >= 0.5f && offSetY >= 0.5f)
            {
                offSetX = 0.0f;
                offSetY = 0.0f;
            }
            else if (offSetX >= 0.5f && offSetY >= 0.0f)
            {
                offSetX = 0.0f;
                offSetY = 0.0f;
            }
            else
                offSetX += offSetChange;
        }
    }

    /*Function: ShowGameImageOnWrongGuess
	 * Purpose: To show a section of the game iamge when someone has a wrong guess
	 * 
	 * Arguments: none
	 * 
	 * Description: Set the currentGameImageBlock active. Then incrememnt the wrongGuessCount
	 */
    private void ShowGameImageOnWrongGuess()
    {
        gameImageBlocks[wrongGuessCount].SetActive(true);
        wrongGuessCount++;
    }

    //SetGameImageActive simply toggles the game objects as visible. 
    /*Fucntion: SetGameImageActive
	 * Purpose: To toggle the gameImageBlocks active or inactive
	 * 
	 * Arguemnts: toggle
	 * 
	 * Description: Iterate through the gameImageBlocks list and toggle each image to the value of the argument to the function
	 */
    void SetGameImageActive(bool toggle)
    {
        foreach (GameObject a in gameImageBlocks)
        {
            a.SetActive(toggle);
        }
    }

    /*Function: CheckWinLose
	 * Purpose: To check for a win or a loss
	 * 
	 * Arguments: none
	 * 
	 * Description: Check for loss first by checking the wrongGuessCount. If it is a loss switch to Loss state.
	 * 				Compare the gameWord to the hiddenword and if they match then switch to win state
	 */
    private void CheckWinLose()
    {
        if (wrongGuessCount == 4)
        {
            SwitchPlayingState(PLAYINGSTATES.loss);
        }
        else if (currentWord.word == hiddenWord)
        {
            SwitchPlayingState(PLAYINGSTATES.win);
            ChangeScore(WIN);
        }
    }
    /*Function: CheckHighScore
	 * Purpose: to see if current endGame beat a high score
	 * 
	 * Arguments: none
	 * 
	 * Description: Iterate through the high score list and see if it beats a high score. if high score list is empty return true
	 */
    private bool CheckHighScore(int score)
    {
        if (highScores.Count == 0)
            return true;
        else
        {
            for (int i = 0; i < highScores.Count; i++)
            {
                if (score > highScores[i].score)
                    return true;
            }

            return false;
        }
    }

    /*Function: UpdateHighScoreList
	 * Purpose: To add the new highscore list entry
	 * 
	 * Arguments: name - score
	 * 
	 * Description: Iterates through the high score list and updates the list of highscores with the new entry
	 */
    private void UpdateHighScoreList(string name, int score)
    {
        if (highScores.Count == highScoreMAX)
            highScores.RemoveAt(highScoreMAX - 1);
        LeaderBoardEntry tempEntry = new LeaderBoardEntry();
        tempEntry.name = name;
        tempEntry.score = score;

        if (highScores.Count == 0)
            highScores.Add(tempEntry);

        for (int i = 0; i < highScores.Count; i++)
        {
            if (score > highScores[i].score)
            {
                highScores.Insert(i, tempEntry);
                break;
            }
        }
    }

    /*Function: FormatHighScore
	 * Purpose: to format a string for the UI to display highscore list
	 * 
	 * Arguments: none
	 * 
	 * Description: iterate through the highscore list and format it to a readable
	 * 				string for the user UI of the leaderboard
	 */
    private void FormatHighScore()
    {
        int x = 1;
        Debug.Log("HighScores Count: " + highScores.Count);
        if (highScores.Count == 0)
            formmatedHighScore = "List is empty";
        else
        {
            const string format = "{0,-4} {1, -20} {2:0000000000}\n";
            formmatedHighScore = string.Format("{0,-4} {1,-20} {2,5}\n\n", "Rank", "Name", "Score");
            foreach (LeaderBoardEntry a in highScores)
            {
                formmatedHighScore += string.Format(format, x, a.name, a.score);
                /*string tempString = x +". " + a.name;
				formmatedHighScore += tempString;
				string whitespace = new string(' ' , 20 - tempString.Length);
				formmatedHighScore += whitespace + a.score + "\n";
				*/
                x++;
            }
        }
    }

    /*Function: LoadHighScores
	 * Purpose: To load the highscore list when application starts
	 * 
	 * Arguments: none
	 * 
	 * Description: open a stream reader and read in the strings from highscore.txt and 
	 * 				parse each line into 2 strings and pass it to the AddHighScoreEntry function
	 */
    private IEnumerator LoadHighScores()
    {
        string line;
        string filename = Application.dataPath + "/HighScore.txt";
        WWW www;

        Debug.Log("Platform: " + Application.platform);
        Debug.Log("Entering LoadHighScores()");

        if (Application.platform.ToString() == "WebGLPlayer")
        {
            www = new WWW(filename);
            yield return www;

            string highScoreString = www.text;

            Debug.Log(highScoreString);

            using (StringReader wwwHighScoreReader = new StringReader(highScoreString))
            {
                do
                {
                    line = wwwHighScoreReader.ReadLine();
                    Debug.Log("Current line: " + line + "-- length: " + line.Length);
                    if (line != null)
                    {
                        string[] higScoreEntry = line.Split(' ');
                        if (higScoreEntry.Length > 0)
                            AddToHighScores(higScoreEntry);
                    }
                } while (line != null);
            }
            FormatHighScore();
        }
        else
        {

            Debug.Log(filename);
            StreamReader highScoreReader = new StreamReader(filename);
            do
            {
                line = highScoreReader.ReadLine();
                if (line != null)
                {
                    string[] highScoreEntry = line.Split(' ');

                    if (highScoreEntry.Length > 0)
                        AddToHighScores(highScoreEntry);
                }
            } while (line != null);

            highScoreReader.Close();
            FormatHighScore();
        }

    }

    /*Function: SaveHighScores
	 * Purprose: TO save the high score list to be loaded the next time the game is played
	 * 
	 * Arguments: none
	 * 
	 * Description: Open a streamwriter and then pull all the data from the high score list and write it to a file
	 */
    private IEnumerator SaveHighScores()
    {
        string filename = Application.dataPath;

        if (Application.platform.ToString() == "WebGLPlayer")
        {
            string sendString = "\n";

            foreach (LeaderBoardEntry a in highScores)
            {
                sendString += a.name + " " + a.score.ToString() + "%0D%0A";
            }

            Debug.Log(sendString);
            WWW uploader = new WWW(filename + "/WriteScript.php?txt=" + sendString);
            yield return uploader;
            Debug.Log("Finished uploading High Scores");

        }
        else
        {
            using (StreamWriter highScoreWriter = new StreamWriter(filename + "/HighScore.txt"))
            {

                foreach (LeaderBoardEntry a in highScores)
                {
                    highScoreWriter.WriteLine(a.name + " " + a.score.ToString());
                }
            }
        }


    }

    /*Function: AddHighScores
	 * Purpose: To take the strings laoded from file and input them into the highscore list.
	 * 
	 * Arguments: HighScoresEntry string array
	 * 
	 * Description: create a leaderboardEntry and then take the 2 strings from the string array argument and input string 1
	 * 				into the name field of the LeaderboardEntry adn then convert the second string to a int and input that value
	 * 				into the leaderboardEntry score field
	 */
    private void AddToHighScores(string[] highScoreEntry)
    {
        LeaderBoardEntry temp = new LeaderBoardEntry();

        temp.name = highScoreEntry[0];
        temp.score = Int32.Parse(highScoreEntry[1]);

        highScores.Add(temp);
    }
    
    /*Function: TimerUpdate
     *Purpose: To update a timer system for game clock
     *
     *Arguments: none
     *
     *Description: if the totalseconds are less then the game length then check the timer being greater then 0 and subtract it by deltatime
     *             when the timer is less then or equal t0 0 then increase totalSeconds by 1 and reset timer to 1.
     */
    void TimerUpdate()
    {
        //Debug.Log ("total seconds: " + totalSeconds + " Delay: " + GameLen + " Timer: " + timer);
        if (totalSeconds < GameLength)
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else if (timer <= 0)
            {
                totalSeconds++;
                timer = 1.0f;
            }
        }
    }

    //=====================================================================================================================================
    //---------------------------------------------- UI Visibility control functions ------------------------------------------------------
    //=====================================================================================================================================

    private void ToggleLossUI()
    {
        float alpha = GameObject.Find("Loss").GetComponent<CanvasGroup>().alpha;
        if (alpha == 0)
        {
            GameObject.Find("Loss").GetComponent<CanvasGroup>().alpha = 1;
        }
        else if (alpha == 1)
        {
            GameObject.Find("Loss").GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    private void ToggleWinUI()
    {
        float alpha = GameObject.Find("Win").GetComponent<CanvasGroup>().alpha;
        if (alpha == 0)
        {
            GameObject.Find("Win").GetComponent<CanvasGroup>().alpha = 1;
        }
        else if (alpha == 1)
        {
            GameObject.Find("Win").GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    private void DimGameUI()
    {
        GameObject.Find("GameControls").GetComponent<CanvasGroup>().alpha = .25f;
    }

    private void BrightenGameUI()
    {
        GameObject.Find("GameControls").GetComponent<CanvasGroup>().alpha = 1;
    }

    private void HideGameUI()
    {
        GameObject.Find("GameControls").GetComponent<CanvasGroup>().alpha = 0;
    }

    private void ToggleEnterNameUI()
    {
        float alpha = GameObject.Find("EnterName").GetComponent<CanvasGroup>().alpha;
        if (alpha == 0)
        {
            GameObject.Find("EnterName").GetComponent<CanvasGroup>().alpha = 1;
        }
        else if (alpha == 1)
        {
            GameObject.Find("EnterName").GetComponent<CanvasGroup>().alpha = 0;
        }
    }
    private void ToggleLeaderBoardUI()
    {
        float alpha = GameObject.Find("Leaderboard").GetComponent<CanvasGroup>().alpha;
        if (alpha == 0)
        {
            GameObject.Find("Leaderboard").GetComponent<CanvasGroup>().alpha = 1;
        }
        else if (alpha == 1)
        {
            GameObject.Find("Leaderboard").GetComponent<CanvasGroup>().alpha = 0;
        }
    }


}
