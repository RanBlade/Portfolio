package ranblade.hearts;

import java.util.*;

public class HeartsState {
	// GameState Data.
	public GameState m_eCurrentGameState;

	public boolean m_bIsHeartsBroken;
	public SuitType m_eCurrentTrickSuit;
	public byte m_byTrickLeadPlayer;

	public byte m_byPlayerCount;
	public byte m_byCurrentPlayerTurn;

	public short m_sRoundCount;
	public short m_sTrickCount;
	public short m_sTrickPlayerTurnCount;

	public List<Card> m_tListGameDeck;
	public List<Card> m_tListCurrentTrickCards;

	public PassDirection m_eNextPassDirection;

	// Player State Data
	// Player 1
	public short m_sPlayer1ID;
	public short m_sPlayer1CurrentTrickScore;
	public short m_sPlayer1TotalScore;
	public String m_stringPlayer1Name;
	public List<Card> m_tListPlayer1Hand;
	public List<Card> m_tListPlayer1UsedCards;
	public List<Card> m_tListPlayer1CardsWon;
	public Card m_tPlayer1TrickCard;
	public List<Card> m_tListPlayer1PassCards;

	// Player 2
	public short m_sPlayer2ID;
	public short m_sPlayer2CurrentTrickScore;
	public short m_sPlayer2TotalScore;
	public String m_stringPlayer2Name;
	public List<Card> m_tListPlayer2Hand;
	public List<Card> m_tListPlayer2UsedCards;
	public List<Card> m_tListPlayer2CardsWon;
	public Card m_tPlayer2TrickCard;
	public List<Card> m_tListPlayer2PassCards;

	// Player 3
	public short m_sPlayer3ID;
	public short m_sPlayer3CurrentTrickScore;
	public short m_sPlayer3TotalScore;
	public String m_stringPlayer3Name;
	public List<Card> m_tListPlayer3Hand;
	public List<Card> m_tListPlayer3UsedCards;
	public List<Card> m_tListPlayer3CardsWon;
	public Card m_tPlayer3TrickCard;
	public List<Card> m_tListPlayer3PassCards;

	// Player 4
	public short m_sPlayer4ID;
	public short m_sPlayer4CurrentTrickScore;
	public short m_sPlayer4TotalScore;
	public String m_stringPlayer4Name;
	public List<Card> m_tListPlayer4Hand;
	public List<Card> m_tListPlayer4UsedCards;
	public List<Card> m_tListPlayer4CardsWon;
	public Card m_tPlayer4TrickCard;
	public List<Card> m_tListPlayer4PassCards;

	public HeartsState() {
		m_eCurrentGameState = GameState.STARTING;
		m_bIsHeartsBroken = false;
		m_eCurrentTrickSuit = null;
		m_byTrickLeadPlayer = -1;
		m_byPlayerCount = 4;
		m_byCurrentPlayerTurn = 0;
		m_sRoundCount = 0;
		m_sTrickCount = 0;
		m_sTrickPlayerTurnCount = 0;
		m_tListGameDeck = new ArrayList<Card>();
		m_tListCurrentTrickCards = new ArrayList<Card>();
		m_eNextPassDirection = PassDirection.NULL;

		m_sPlayer1ID = 1;
		m_sPlayer1CurrentTrickScore = 0;
		m_sPlayer1TotalScore = 0;
		m_stringPlayer1Name = "None";
		m_tListPlayer1Hand = new ArrayList<Card>();
		m_tListPlayer1CardsWon = new ArrayList<Card>();
		m_tListPlayer1PassCards = new ArrayList<Card>();
		m_tPlayer1TrickCard = new Card();

		m_sPlayer2ID = 2;
		m_sPlayer2CurrentTrickScore = 0;
		m_sPlayer2TotalScore = 0;
		m_stringPlayer2Name = "None";
		m_tListPlayer2Hand = new ArrayList<Card>();
		m_tListPlayer2CardsWon = new ArrayList<Card>();
		m_tListPlayer2PassCards = new ArrayList<Card>();
		m_tPlayer2TrickCard = new Card();

		m_sPlayer3ID = 3;
		m_sPlayer3CurrentTrickScore = 0;
		m_sPlayer3TotalScore = 0;
		m_stringPlayer3Name = "None";
		m_tListPlayer3Hand = new ArrayList<Card>();
		m_tListPlayer3CardsWon = new ArrayList<Card>();
		m_tListPlayer3PassCards = new ArrayList<Card>();
		m_tPlayer3TrickCard = new Card();

		m_sPlayer4ID = 4;
		m_sPlayer4CurrentTrickScore = 0;
		m_sPlayer4TotalScore = 0;
		m_stringPlayer4Name = "None";
		m_tListPlayer4Hand = new ArrayList<Card>();
		m_tListPlayer4CardsWon = new ArrayList<Card>();
		m_tListPlayer4PassCards = new ArrayList<Card>();
		m_tPlayer4TrickCard = new Card();
		
		m_tListPlayer1UsedCards = new ArrayList<Card>();
		m_tListPlayer2UsedCards = new ArrayList<Card>();
		m_tListPlayer3UsedCards = new ArrayList<Card>();
		m_tListPlayer4UsedCards = new ArrayList<Card>();

	}
};

/*
 * public class HeartsState {
 * 
 * //current game state variables public GameState m_eCurrentGameState; public
 * int m_iCurrentRound; public int m_iHighestScore; public int
 * m_iCurrentPlayerTurn; public int m_iPlayerStartTurn; public PassDirection
 * m_eCurrentPass; public List<PlayerStateInfo> m_tListPlayersState;
 * 
 * public boolean m_bHeartsBroken;
 * 
 * //constant game state variables public int m_iPlayerCount; public int
 * m_iMaxPlayers;
 * 
 * public HeartsState() { m_eCurrentGameState = GameState.NEWGAME;
 * m_iCurrentRound = 0; m_iHighestScore = 0; m_iCurrentPlayerTurn = -1;
 * m_iPlayerStartTurn = -1; m_eCurrentPass = PassDirection.LEFT;
 * m_tListPlayersState = new ArrayList<PlayerStateInfo>(); //deckState = new
 * ArrayList<CardState>();
 * 
 * m_iPlayerCount = 0; m_iMaxPlayers = 4;
 * 
 * m_bHeartsBroken = false; }
 * 
 * 
 * 
 * }
 */
