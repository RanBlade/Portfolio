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


	// word variables for the game state
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
		currentGstate = PlayingStates.newGame;
		//create memory for the gamestate containeers
		gameImageBlocks = new List<GameObject>();
		guessedLetters = new List<char>();
		currentWord = new WordLibEntry();

		//Initiliaze all other game state variables
		pickWord = true;
		playerGuessed = false;
		guessCount = 0;
		wrongGuessCount = 0;
		roundCount = 0;
		gameScore = 0;
		currentWord = GetComponent<WordLib>().GetWord();
		SetHiddenWord ();


		Debug.Log ("CurrentWord: " + currentWord.word + " Size: " + currentWord.GetWordSize());

		//Setup functions to setup initial gamestate or do testing
		CreateGameImage();
		SetGameImageActive(false);
		LoadGameImage();
		SetGameImage();
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
				case PlayingStates.playerTurn:
				{
					if (playerGuessed) {
						ConvertGuessedLettersToString ();
					
						playerGuessed = false;
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

	//This function creates the blocks that the game image will be applied to and that the game will use to give hints to the player 
	//if they get wrong guesses.
	void CreateGameImage()
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
	void LoadGameImage()
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

	//SetGameImageActive simply toggles the game objects as visible. 
	void SetGameImageActive(bool toggle)
	{
		foreach(GameObject a in gameImageBlocks)
		{
			a.SetActive(toggle);
		}
	}
	void ShowGameImageOnWrongGuess()
	{
		gameImageBlocks[wrongGuessCount].SetActive(true);
		wrongGuessCount++;
	}

	//Loads the texture onto the game blocks
	void SetGameImage()
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

	void UpdateUI()
	{
	}


	//Public accessor fucntions for other objects like UI to interface with the game state
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

		if(!CheckifLetterUsed(tempText) && char.IsLetter(tempText))
		{
			if(CheckGuess(tempText))
			    RightGuess(tempText);
			else
				WrongGuess (tempText);
		}
		else{
		//Add logic to handle letting the user know they have already guessed this letter OR they made a wrong selection. 
		}


		playerGuessed = true;
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
	}
	private void WrongGuess(char incorrectGuess)
	{
		ShowGameImageOnWrongGuess ();
		guessedLetters.Add(incorrectGuess);

	}
	private void SetHiddenWord()
	{
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
	}


}
