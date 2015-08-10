using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UtilityScripts;

public class gameState : MonoBehaviour {

	//enum for game states
	enum MainStates
	{
		menu,
		playing,
		quit,
		pause,
		loading
	}
	enum PlayingStates
	{
		playerTurn,
		win,
		loss,
		newGame,
		mainMenu
	}

	//game counters
	private int roundCount, guessCount, wrongGuessCount, gameScore;

	//game true/false conditions
	private bool playerGuessed;
	private bool pickWord;
	private bool enterState;
	private bool playAgain;

	// word variables for the game state
	private char guessedLetter;
	private List<char> guessedLetters;
	private string formattedGuessedLetters;
	private string hiddenWord;
	private WordLibEntry currentWord;

	//stored list of the gameblocks for the image
	private List<GameObject> gameImageBlocks;
	private Texture gameImage;

	//current States
	private MainStates currentMstate;
	private PlayingStates currentGstate;


	// Use this for initialization
	void Start () {
		//fix this after testing to proper start states
		currentMstate = MainStates.playing;
		currentGstate = PlayingStates.playerTurn;

		//create memory for the gamestate containeers
		gameImageBlocks = new List<GameObject>();
		guessedLetters = new List<char>();
		currentWord = new WordLibEntry();

		CreateGameImage();
		LoadGameImage();

		enterState = false;
		playAgain = false;

		SwitchPlayingState(PlayingStates.newGame);
	
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentMstate) {
		
		case MainStates.menu:
		{
			break;
		}
		case MainStates.loading:
		{
			break;
		}
		case MainStates.playing:
		{
			switch(currentGstate)
			{
			case PlayingStates.newGame:
			{
				if(enterState)
				{
					StartNewGame();
					enterState = false;
					SwitchPlayingState(PlayingStates.playerTurn);
				}

				break;
			}
			case PlayingStates.playerTurn:
			{
				if(enterState)
				{
					enterState = false;
				}
				if (playerGuessed) {
					ProcessGuess();
					ConvertGuessedLettersToString ();
					CheckWinLose ();
					playerGuessed = false;
				}
				break;
			}
			case PlayingStates.mainMenu:
			{
				break;
			}
			case PlayingStates.win:
			{
				if(enterState){
					//GameObject.Find ("Win").GetComponent<CanvasGroup>().alpha = 1;
					ToggleWinUI();
					Debug.Log ("YOU WIN!!! CONGRATS!");
					enterState = false;
				}
				if(playAgain)
				{
					SwitchPlayingState(PlayingStates.newGame);
					ToggleWinUI();
					playAgain = false;
				}
				break;
			}
			case PlayingStates.loss:
			{
				if(enterState)
				{
					//GameObject.Find("Loss").GetComponent<CanvasGroup>().alpha = 1;
					ToggleLossUI();
					Debug.Log ("You Lose! Sorry try again");
					enterState = false;
				}
				if(playAgain)
				{
					SwitchPlayingState(PlayingStates.newGame);
					ToggleLossUI();
					playAgain = false;
				}
				break;
			}
			}
			break;
		}
		case MainStates.pause:
		{
			break;
		}
		case MainStates.quit:
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
	public int GetScore()
	{
		return gameScore;
	}

	/*Name: Guess
	 *Purpose: To handle what the game does when a player hits the Guess button
	 *
	 *Arguments: 
	 *		char tempText: The input from the player of the letter they guessed
	 *
	 *Description: The fucntion takes the input tempText. It then test tempText to make sure it is a letter. 
	 *				If it is a letter then it test tempText against the letters that have already been guessed
	 *				if the letter has not been guessed. It will check the guess for being a right guess or wrong. 
	 *				if it is wrong it will call the wrong guess fucntion and the right guess fucntion if it is right.
	 */
	public void Guess(char tempText)
	{
		Debug.Log ("Testing Click and Guess: " + tempText + "!!!");
		guessedLetter = tempText;
		playerGuessed = true;

	}

	/*Function: QuitGame
	 * Purpose: to clean quit the game when the player presses Quit Game button
	 * 
	 * Arguments: none
	 * 
	 * Description: Clean up game elements and save any data that needs to be saved then quit the game
	 */
	public void QuitGame()
	{
		Application.Quit();
	}

	public void PlayAgain()
	{
		playAgain = true;
	}
	public void PlayerNewGame()
	{
	}
	public void ShowLeaderBoard()
	{
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
		if(!CheckifLetterUsed(guessedLetter) && char.IsLetter(guessedLetter))
		{
			if(CheckGuess(guessedLetter))
				RightGuess(guessedLetter);
			else
				WrongGuess (guessedLetter);
		}
		else{
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
	private bool CheckifLetterUsed( char tempLetter )
	{
		foreach(char c in guessedLetters)
		{
			if(c == tempLetter)
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
		foreach(char c in guessedLetters)
		{
			formattedGuessedLetters+= (c + ", ");
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
		foreach(char c in currentWord.word)
		{
			if(guessLetter == c || guessLetter == char.ToLower(c))
			{
				Debug.Log ("YOU GOT A LETTER!!!");
				return true;
			}
		}
		Debug.Log ("Wrong Guess");
		return false;
	}
	private void RightGuess(char correctGuess)
	{
		guessedLetters.Add (correctGuess);
		ShowHiddenLetter (correctGuess);
		ChangeScore(15);
	}
	private void WrongGuess(char incorrectGuess)
	{
		ShowGameImageOnWrongGuess ();
		guessedLetters.Add(incorrectGuess);
		ChangeScore(-5);

	}
	private void ChangeScore(int score)
	{
		gameScore+=score;
	}
	private void SetHiddenWord()
	{
		hiddenWord = null;
		foreach (char c in currentWord.word) {
			hiddenWord+= "_";
		}
	}
	private void ShowHiddenLetter(char guessedLetter)
	{
		int stringIndex = 0;
		char[] tempArray = hiddenWord.ToCharArray ();
		foreach (char c in currentWord.word) {
			Debug.Log ("c: " + c + " guessedLetter: " + guessedLetter);
			if(c == guessedLetter || guessedLetter == char.ToLower(c))
			{
				Debug.Log ("Found match " + c);
				tempArray[stringIndex] = c;
			}
			stringIndex++;
		}
		hiddenWord = new string (tempArray);
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
		pickWord = true;
		playerGuessed = false;
		guessCount = 0;
		wrongGuessCount = 0;
		roundCount = 0;
		gameScore = 0;

		currentWord = GetComponent<WordLib>().GetWord();
		SetHiddenWord ();

		if(guessedLetters.Count > 0)
			guessedLetters.Clear();
		//if(formattedGuessedLetters.Length > 0)
			formattedGuessedLetters = " ";
		
		Debug.Log ("CurrentWord: " + currentWord.word + " Size: " + currentWord.GetWordSize());
		
		//Setup functions to setup initial gamestate or do testing
		SetGameImage();
		SetGameImageActive(false);
	}

	/*Function: SwitchPlayingState
	 * Purpose: To switch between states
	 * 
	 * arguments: tempState
	 * 
	 * Description: Take state argument and assign to current playing State
	 */
	private void SwitchPlayingState(PlayingStates tempState)
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
	private void SwitchMainState(MainStates tempState)
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
		
		
		gameImageBlocks.Add (topLeft);
		gameImageBlocks.Add (topRight);
		gameImageBlocks.Add (bottomLeft);
		gameImageBlocks.Add (bottomRight);
		
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
		int yPos =0;
		int zPos = 300;
		int incrementVal = 25;
		int incrementScale = 25;
		int zRot = 180; //Im not a art guy I don't know why but the iamge has to be rotated 180 on the z axis to be right side up --maybe ill fix later
		
		foreach(GameObject a in gameImageBlocks)
		{
			Vector3 tempPos = new Vector3(xPos, yPos , zPos);
			
			a.transform.position = tempPos;	
			a.transform.Rotate(0,0, zRot);
			a.transform.localScale += new Vector3( incrementScale, incrementScale, 1);
			
			xPos+=incrementVal;
			if(xPos >= 50)
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
		
		if(gameImage == null)
			Debug.Log ("Game Image is NULL");
		
		foreach( GameObject block in gameImageBlocks)
		{
			block.GetComponent<MeshRenderer>().material.mainTexture = gameImage;
			block.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex" , new Vector2( 0.5f , 0.5f));
			block.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex" , new Vector2(offSetX , offSetY));
			
			if(offSetX >= 0.5f && offSetY >= 0.5f){
				offSetX = 0.0f;
				offSetY = 0.0f;
			}
			else if(offSetX >= 0.5f && offSetY >= 0.0f)
			{
				offSetX = 0.0f;
				offSetY = 0.0f;
			}
			else
				offSetX+=offSetChange;
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
		foreach(GameObject a in gameImageBlocks)
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
		if(wrongGuessCount == 4)
		{
			SwitchPlayingState(PlayingStates.loss);
		}
		else if( currentWord.word == hiddenWord)
		{
			SwitchPlayingState(PlayingStates.win);
			ChangeScore(100);
		}
	}

	private void ToggleLossUI()
	{
		float alpha = GameObject.Find("Loss").GetComponent<CanvasGroup>().alpha;
		if(alpha == 0)
		{
			GameObject.Find("Loss").GetComponent<CanvasGroup>().alpha = 1;
		}
		else if(alpha == 1)
		{
			GameObject.Find("Loss").GetComponent<CanvasGroup>().alpha = 0;
		}
	}

	private void ToggleWinUI()
	{
		float alpha = GameObject.Find("Win").GetComponent<CanvasGroup>().alpha;
		if(alpha == 0)
		{
			GameObject.Find("Win").GetComponent<CanvasGroup>().alpha = 1;
		}
		else if(alpha == 1)
		{
			GameObject.Find("Win").GetComponent<CanvasGroup>().alpha = 0;
		}
	}
	private void DimGameUI()
	{
		GameObject.Find ("GameControls").GetComponent<CanvasGroup>().alpha = .25f;
	}
	private void BrightenGameUI()
	{
		GameObject.Find ("GameControls").GetComponent<CanvasGroup>().alpha = 1;
	}
	private void HideGameUI()
	{
		GameObject.Find ("GameControls").GetComponent<CanvasGroup>().alpha = 0;
	}

}
