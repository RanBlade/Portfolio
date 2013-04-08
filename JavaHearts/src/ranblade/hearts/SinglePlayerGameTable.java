package ranblade.hearts;

import ranblade.game.*;
import java.io.*;
import java.util.*;

/* HARDCODED FOR NOW...
 * GAME TABLE 
 *                Player 3
 *    Player 2                 Player 4
 *                Player 1
 */


public class SinglePlayerGameTable {
	private static class GameMode
	{
		public static boolean Debug = true;
	}
	// All Players
	private short m_sServerID = 1001;
	private HeartsState m_tMasterGameState;
	private MessageHandler m_tMessageHandler = new MessageHandler(HandlerType.SERVER , m_sServerID );
	private AIPlayer m_tPlayer1;
	private AIPlayer m_tPlayer2;
	private AIPlayer m_tPlayer3;
	private AIPlayer m_tPlayer4;

	/* *****************************************************************************
	 * MEMBER FUCNTIONS FOR USE BY THE GAMEMANAGER CLASS FOR USE WHILE MANAGING
	 * THE GAME
	 * ******************************************************************
	 * ***********
	 */
	public SinglePlayerGameTable(AIPlayer t_player1 , AIPlayer t_player2, AIPlayer t_player3, AIPlayer t_player4) {
		m_tMasterGameState = new HeartsState();
		m_tPlayer1 = t_player1;
		m_tPlayer2 = t_player2;
		m_tPlayer3 = t_player3;
		m_tPlayer4 = t_player4;

	}

	// Function name: Update
	// PARAMS: none
	// Purpose: To update the game as players play.
	public boolean Update( ) {
		switch ( m_tMasterGameState.m_eCurrentGameState ) {
		case STARTING: {
			StartNewRound();
			SetupGameTable();
			m_tMasterGameState.m_eCurrentGameState = GameState.DEALING;
		}
		case DEALING: {
			DealCards();

			SycnronizePlayerStates();
			m_tMasterGameState.m_eCurrentGameState = GameState.PASSINGCARDS;
		}
		case PASSINGCARDS: 
				return HandlePassCardsState();
		case PLAYING: 
				return HandlePlayingState();
		case ENDING: {
			return false;
		}
		}
		SycnronizePlayerStates();
		return false;
	}

	private void SycnronizePlayerStates( ) {
		m_tPlayer1.m_tAIPlayerGameState = m_tMasterGameState;
		m_tPlayer2.m_tAIPlayerGameState = m_tMasterGameState;
		m_tPlayer3.m_tAIPlayerGameState = m_tMasterGameState;
		m_tPlayer4.m_tAIPlayerGameState = m_tMasterGameState;
	}

	// Function name: SetupGameTable
	// PARAMS: none
	// Purpose: Setup the cards and players at the table.. graphics wise
	private void SetupGameTable( ) {
		SycnronizePlayerStates();
		GenerateGameDeck();

		m_tPlayer1.m_sAIPlayerID = m_tMasterGameState.m_sPlayer1ID;
		m_tPlayer2.m_sAIPlayerID = m_tMasterGameState.m_sPlayer2ID;
		m_tPlayer3.m_sAIPlayerID = m_tMasterGameState.m_sPlayer3ID;
		m_tPlayer4.m_sAIPlayerID = m_tMasterGameState.m_sPlayer4ID;
		if(m_tMasterGameState.m_sRoundCount == 1 )
			m_tMasterGameState.m_eNextPassDirection = PassDirection.LEFT;
		m_tMasterGameState.m_byCurrentPlayerTurn = Constant.PLAYER_ONE;
		//m_tMasterGameState.m_sRoundCount = 1;
		m_tMasterGameState.m_sTrickCount = 1;
		m_tMasterGameState.m_sTrickPlayerTurnCount = 1;
	}

	// Function name: DealCards
	// PARAMS: none
	// Purpose: To deal the cards to the players.
	private void DealCards( ) {
		int t_count = 1;
		Random t_generator = new Random();

		while ( !m_tMasterGameState.m_tListGameDeck.isEmpty() ) {
			if ( t_count > 4 )
				t_count = 1;
			if ( t_count == 1 ) {
				int t_cardNum = t_generator
						.nextInt( m_tMasterGameState.m_tListGameDeck.size() );
				m_tMasterGameState.m_tListPlayer1Hand
						.add( m_tMasterGameState.m_tListGameDeck
								.get( t_cardNum ) );
				m_tMasterGameState.m_tListGameDeck.remove( t_cardNum );
			}
			if ( t_count == 2 ) {
				int t_cardNum = t_generator
						.nextInt( m_tMasterGameState.m_tListGameDeck.size() );
				m_tMasterGameState.m_tListPlayer2Hand
						.add( m_tMasterGameState.m_tListGameDeck
								.get( t_cardNum ) );
				m_tMasterGameState.m_tListGameDeck.remove( t_cardNum );
			}
			if ( t_count == 3 ) {
				int t_cardNum = t_generator
						.nextInt( m_tMasterGameState.m_tListGameDeck.size() );
				m_tMasterGameState.m_tListPlayer3Hand
						.add( m_tMasterGameState.m_tListGameDeck
								.get( t_cardNum ) );
				m_tMasterGameState.m_tListGameDeck.remove( t_cardNum );
			}
			if ( t_count == 4 ) {
				int t_cardNum = t_generator
						.nextInt( m_tMasterGameState.m_tListGameDeck.size() );
				m_tMasterGameState.m_tListPlayer4Hand
						.add( m_tMasterGameState.m_tListGameDeck
								.get( t_cardNum ) );
				m_tMasterGameState.m_tListGameDeck.remove( t_cardNum );
			}

			t_count++;

		}
		if( GameMode.Debug )
			System.out.println( "Cards dealt to all players " );
	}

	private void GenerateGameDeck( ) {
		int t_count = 1;
		if ( m_tMasterGameState.m_tListGameDeck != null )
			m_tMasterGameState.m_tListGameDeck.clear();

		for ( SuitType s : SuitType.values() ) {
			for ( CardType c : CardType.values() ) {
				Card t_card = new Card();
				t_card.m_iCardID = t_count;
				t_card.m_tSuitValue = s;
				t_card.m_tCardValue = c;
				m_tMasterGameState.m_tListGameDeck.add( t_card );
				
				t_count++;
			}
		}
		if( GameMode.Debug )
			System.out.println( " Game Deck Generated " );
	}

	/* *****************************************************************************
	 * MEMBER FUCNTIONS FOR USE BY THE PLAYERS TO USE TO PLAY THE GAME.
	 * **********
	 * *******************************************************************
	 */

	/* *****************************************************************************
	 * MEMBER FUCNTIONS FOR USE BY THE GAMEMANAGER CLASS TO MAKE THE CODE MORE
	 * READABLE
	 * 
	 * **************************************************************************
	 * ***
	 */
	private void SetNextPlayerTurn( ) {
		m_tMasterGameState.m_byCurrentPlayerTurn++;
		m_tMasterGameState.m_sTrickPlayerTurnCount++;
		if ( m_tMasterGameState.m_byCurrentPlayerTurn > Constant.PLAYER_FOUR )
			m_tMasterGameState.m_byCurrentPlayerTurn = Constant.PLAYER_ONE;
	}

	private boolean CheckPlayerHasCard( short t_playerID , Card t_card ) {
		if ( t_playerID == 1 ) {
			// System.out.println( " Testing Player1's card for accuracy" );
			for ( Card t_testCard : m_tMasterGameState.m_tListPlayer1Hand ) {
				if ( t_testCard.m_tCardValue == t_card.m_tCardValue
						&& t_testCard.m_tSuitValue == t_card.m_tSuitValue ) {
					return true;
				}
			}
		} else if ( t_playerID == 2 ) {
			// System.out.println( " Testing Player2's card for accuracy" );
			for ( Card t_testCard : m_tMasterGameState.m_tListPlayer2Hand ) {
				if ( t_testCard.m_tCardValue == t_card.m_tCardValue
						&& t_testCard.m_tSuitValue == t_card.m_tSuitValue ) {
					return true;
				}
			}
		} else if ( t_playerID == 3 ) {
			// System.out.println( " Testing Player3's card for accuracy" );
			for ( Card t_testCard : m_tMasterGameState.m_tListPlayer3Hand ) {
				if ( t_testCard.m_tCardValue == t_card.m_tCardValue
						&& t_testCard.m_tSuitValue == t_card.m_tSuitValue ) {
					return true;
				}
			}
		} else if ( t_playerID == 4 ) {
			// System.out.println( " Testing Player4's card for accuracy" );
			for ( Card t_testCard : m_tMasterGameState.m_tListPlayer4Hand ) {
				if ( t_testCard.m_tCardValue == t_card.m_tCardValue
						&& t_testCard.m_tSuitValue == t_card.m_tSuitValue ) {
					return true;
				}
			}
		}
		return false;
	}
	private boolean AllPlayersHaveSameSizeHand()
	{
		if( (m_tMasterGameState.m_tListPlayer1Hand.size() + m_tMasterGameState.m_tListPlayer2Hand.size() + 
			m_tMasterGameState.m_tListPlayer3Hand.size() + m_tMasterGameState.m_tListPlayer4Hand.size())/ 4 == m_tMasterGameState.m_tListPlayer1Hand.size() )
		{
			return true;
			
		}
		else 
			return false;
	}
	private boolean PlayersHaveZeroCards( ) {
		if ( m_tMasterGameState.m_tListPlayer1Hand.size() == 0
				&& m_tMasterGameState.m_tListPlayer2Hand.size() == 0
				&& m_tMasterGameState.m_tListPlayer3Hand.size() == 0
				&& m_tMasterGameState.m_tListPlayer4Hand.size() == 0 )
			return true;

		else
			return false;
	}

	private int FindPlayerWithTwoOfClubs( ) {
		Card t_card = new Card();
		t_card.m_tCardValue = CardType.DUECE;
		t_card.m_tSuitValue = SuitType.CLUBS;

		if ( CheckPlayerHasCard( m_tMasterGameState.m_sPlayer1ID , t_card ) )
			return m_tMasterGameState.m_sPlayer1ID;
		else if ( CheckPlayerHasCard( m_tMasterGameState.m_sPlayer2ID , t_card ) )
			return m_tMasterGameState.m_sPlayer2ID;
		else if ( CheckPlayerHasCard( m_tMasterGameState.m_sPlayer3ID , t_card ) )
			return m_tMasterGameState.m_sPlayer3ID;
		else if ( CheckPlayerHasCard( m_tMasterGameState.m_sPlayer4ID , t_card ) )
			return m_tMasterGameState.m_sPlayer4ID;
		else
			return -1;
	}

	private boolean CheckTrickRulesOnCard( short t_player , Card t_card ) {
		// Doing this for any comparisions we may need to check on the whole
		// deck
		List<Card> t_playerHand = new ArrayList<Card>();
		if ( t_player == Constant.PLAYER_ONE )
			t_playerHand = m_tMasterGameState.m_tListPlayer1Hand;
		else if ( t_player == Constant.PLAYER_TWO )
			t_playerHand = m_tMasterGameState.m_tListPlayer2Hand;
		else if ( t_player == Constant.PLAYER_THREE )
			t_playerHand = m_tMasterGameState.m_tListPlayer3Hand;
		else if ( t_player == Constant.PLAYER_FOUR )
			t_playerHand = m_tMasterGameState.m_tListPlayer4Hand;
		else
			// we should never hit this else statement.
			return false;

		if ( t_player == m_tMasterGameState.m_byCurrentPlayerTurn ) {
			// If all players have 13 cards then we assume beggining of round..
			// make
			// sure the player has played 2 of clubs.
			// I assume for checking random things like hand size i can do what
			// i do below...
			if ( m_tMasterGameState.m_sTrickCount == Constant.FIRST_TRICK_OF_ROUND 
					&& m_tMasterGameState.m_sTrickPlayerTurnCount == Constant.FIRST_TURN_OF_TRICK ) {
				if ( t_card.m_tCardValue == CardType.DUECE
						&& t_card.m_tSuitValue == SuitType.CLUBS )
					return true;
				else
					return false;
			}
			//if hearts isnt broken and its the lead trick player then cant play heart
			else if( m_tMasterGameState.m_bIsHeartsBroken == false &&
					 m_tMasterGameState.m_byTrickLeadPlayer == t_player &&
					 t_card.m_tSuitValue == SuitType.HEARTS )
				return false;
			// Now we need to check if the card played is a heart and if hearts
			// isnt broken
			else if ( t_card.m_tSuitValue == SuitType.HEARTS
					&& m_tMasterGameState.m_bIsHeartsBroken == false ) {
				// so now that we know which player hand we working on lets
				// check
				// to see if they have any of current trick suit.. if so we
				// return false
				// if they dont we return true.
				for ( Card c : t_playerHand ) {
					if ( c.m_tSuitValue == m_tMasterGameState.m_eCurrentTrickSuit )
						return false;
				}
				// if we made it here then the player does not have any cards
				// matching the trick suit so we can return true so the heart
				// card
				// can be played
				m_tMasterGameState.m_bIsHeartsBroken = true;
				return true;
			}
			// Now that we know about the hearts and if its broken or not
			// we need to know if the card differs in suit then the current
			// leading suit
			// if so we need to verify they dont have leading suit in hand.
			else if ( t_card.m_tSuitValue != m_tMasterGameState.m_eCurrentTrickSuit ) {
				for ( Card c : t_playerHand ) {
					if ( c.m_tSuitValue == m_tMasterGameState.m_eCurrentTrickSuit )
						return false;
				}
				// player really doesnt have said card suit in his deck. so
				// return true
				return true;
			}
			// Check if the card matches the current trick suit if so then just
			// return true.
			else if ( t_card.m_tSuitValue == m_tMasterGameState.m_eCurrentTrickSuit )
				return true;
			else
				return false; // again should never hit this statement...

		}
		return false;
	}

	private void AddCardToPlayerTrickCard( short t_id , Card t_trickCard ) {
		if ( t_id == Constant.PLAYER_ONE )
			m_tMasterGameState.m_tPlayer1TrickCard = t_trickCard;
		if ( t_id == Constant.PLAYER_TWO )
			m_tMasterGameState.m_tPlayer2TrickCard = t_trickCard;
		if ( t_id == Constant.PLAYER_THREE )
			m_tMasterGameState.m_tPlayer3TrickCard = t_trickCard;
		if ( t_id == Constant.PLAYER_FOUR )
			m_tMasterGameState.m_tPlayer4TrickCard = t_trickCard;

	}

	private void AddCardsToPlayerDeck( short t_id , List<Card> t_cards ) {
		if ( t_id == Constant.PLAYER_ONE ) {
			m_tMasterGameState.m_tListPlayer1Hand.addAll( t_cards );
		} else if ( t_id == Constant.PLAYER_TWO ) {
			m_tMasterGameState.m_tListPlayer2Hand.addAll( t_cards );
		} else if ( t_id == Constant.PLAYER_THREE ) {
			m_tMasterGameState.m_tListPlayer3Hand.addAll( t_cards );
		} else if ( t_id == Constant.PLAYER_FOUR ) {
			m_tMasterGameState.m_tListPlayer4Hand.addAll( t_cards );
		}
	}

	private void RemoveCardFromPlayer( short t_id , Card t_card ) {
		if ( t_id == Constant.PLAYER_ONE ) {
			m_tMasterGameState.m_tListPlayer1Hand.remove( t_card );
		} else if ( t_id == Constant.PLAYER_TWO ) {
			m_tMasterGameState.m_tListPlayer2Hand.remove( t_card );
		} else if ( t_id == Constant.PLAYER_THREE ) {
			m_tMasterGameState.m_tListPlayer3Hand.remove( t_card );
		} else if ( t_id == Constant.PLAYER_FOUR ) {
			m_tMasterGameState.m_tListPlayer4Hand.remove( t_card );
		}
	}

	private boolean AllPlayersHavePlayedTrick( ) {
		if ( m_tMasterGameState.m_tPlayer1TrickCard.m_tSuitValue != null
				&& m_tMasterGameState.m_tPlayer2TrickCard.m_tSuitValue != null
				&& m_tMasterGameState.m_tPlayer3TrickCard.m_tSuitValue != null
				&& m_tMasterGameState.m_tPlayer4TrickCard.m_tSuitValue != null )
			return true;
		else
			return false;
	}
	private boolean AllPlayersHavePassedCards()
	{
		if( m_tMasterGameState.m_tListPlayer1PassCards.size() == Constant.PASS_CARD_COUNT &&
				m_tMasterGameState.m_tListPlayer2PassCards.size() == Constant.PASS_CARD_COUNT &&
				m_tMasterGameState.m_tListPlayer3PassCards.size() == Constant.PASS_CARD_COUNT &&
				m_tMasterGameState.m_tListPlayer4PassCards.size() == Constant.PASS_CARD_COUNT )
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private short CalculateTrickPointValue( ) {
		short t_score = 0;
		if ( m_tMasterGameState.m_tPlayer1TrickCard.m_tSuitValue == SuitType.HEARTS )
			t_score++;
		if ( m_tMasterGameState.m_tPlayer2TrickCard.m_tSuitValue == SuitType.HEARTS )
			t_score++;
		if ( m_tMasterGameState.m_tPlayer3TrickCard.m_tSuitValue == SuitType.HEARTS )
			t_score++;
		if ( m_tMasterGameState.m_tPlayer4TrickCard.m_tSuitValue == SuitType.HEARTS )
			t_score++;

		if ( m_tMasterGameState.m_tPlayer1TrickCard.m_tSuitValue == SuitType.SPAIDS
				&& m_tMasterGameState.m_tPlayer1TrickCard.m_tCardValue == CardType.QUEEN )
			t_score += Constant.QUEEN_SPAIDS_POINT_VALUE;
		if ( m_tMasterGameState.m_tPlayer2TrickCard.m_tSuitValue == SuitType.SPAIDS
				&& m_tMasterGameState.m_tPlayer2TrickCard.m_tCardValue == CardType.QUEEN )
			t_score += Constant.QUEEN_SPAIDS_POINT_VALUE;
		if ( m_tMasterGameState.m_tPlayer3TrickCard.m_tSuitValue == SuitType.SPAIDS
				&& m_tMasterGameState.m_tPlayer3TrickCard.m_tCardValue == CardType.QUEEN )
			t_score += Constant.QUEEN_SPAIDS_POINT_VALUE;
		if ( m_tMasterGameState.m_tPlayer4TrickCard.m_tSuitValue == SuitType.SPAIDS
				&& m_tMasterGameState.m_tPlayer4TrickCard.m_tCardValue == CardType.QUEEN )
			t_score += Constant.QUEEN_SPAIDS_POINT_VALUE;

		return t_score;
	}

	private void FindTrickWinnerAndAssignPoints( short t_score ) {
		short t_CurrentHighCardPlayerID = m_tMasterGameState.m_byTrickLeadPlayer;
		Card t_HighCard = new Card();
		// set the starting trick player card to the high card.
		if ( t_CurrentHighCardPlayerID == m_tMasterGameState.m_sPlayer1ID )
			t_HighCard = m_tMasterGameState.m_tPlayer1TrickCard;
		else if ( t_CurrentHighCardPlayerID == m_tMasterGameState.m_sPlayer2ID )
			t_HighCard = m_tMasterGameState.m_tPlayer2TrickCard;
		else if ( t_CurrentHighCardPlayerID == m_tMasterGameState.m_sPlayer3ID )
			t_HighCard = m_tMasterGameState.m_tPlayer3TrickCard;
		else if ( t_CurrentHighCardPlayerID == m_tMasterGameState.m_sPlayer4ID )
			t_HighCard = m_tMasterGameState.m_tPlayer4TrickCard;

		if ( m_tMasterGameState.m_tPlayer1TrickCard.m_tSuitValue == t_HighCard.m_tSuitValue
				&& m_tMasterGameState.m_tPlayer1TrickCard.m_tCardValue
						.ordinal() > t_HighCard.m_tCardValue.ordinal() ) {
			t_HighCard = m_tMasterGameState.m_tPlayer1TrickCard;
			t_CurrentHighCardPlayerID = m_tMasterGameState.m_sPlayer1ID;
		}
		if ( m_tMasterGameState.m_tPlayer2TrickCard.m_tSuitValue == t_HighCard.m_tSuitValue
				&& m_tMasterGameState.m_tPlayer2TrickCard.m_tCardValue
						.ordinal() > t_HighCard.m_tCardValue.ordinal() ) {
			t_HighCard = m_tMasterGameState.m_tPlayer2TrickCard;
			t_CurrentHighCardPlayerID = m_tMasterGameState.m_sPlayer2ID;
		}
		if ( m_tMasterGameState.m_tPlayer3TrickCard.m_tSuitValue == t_HighCard.m_tSuitValue
				&& m_tMasterGameState.m_tPlayer3TrickCard.m_tCardValue
						.ordinal() > t_HighCard.m_tCardValue.ordinal() ) {
			t_HighCard = m_tMasterGameState.m_tPlayer3TrickCard;
			t_CurrentHighCardPlayerID = m_tMasterGameState.m_sPlayer3ID;
		}
		if ( m_tMasterGameState.m_tPlayer4TrickCard.m_tSuitValue == t_HighCard.m_tSuitValue
				&& m_tMasterGameState.m_tPlayer4TrickCard.m_tCardValue
						.ordinal() > t_HighCard.m_tCardValue.ordinal() ) {
			t_HighCard = m_tMasterGameState.m_tPlayer4TrickCard;
			t_CurrentHighCardPlayerID = m_tMasterGameState.m_sPlayer4ID;
		}

		if ( t_CurrentHighCardPlayerID == m_tMasterGameState.m_sPlayer1ID ) {
			m_tMasterGameState.m_sPlayer1CurrentTrickScore += t_score;
			m_tMasterGameState.m_byTrickLeadPlayer = (byte)m_tMasterGameState.m_sPlayer1ID;
			m_tMasterGameState.m_byCurrentPlayerTurn = (byte)m_tMasterGameState.m_sPlayer1ID;
			if ( m_tMasterGameState.m_sPlayer1CurrentTrickScore == Constant.MAX_SCORE_VALUE) 
			{
				m_tMasterGameState.m_sPlayer2CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer3CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer4CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer1CurrentTrickScore = 0;
			}
		} else if ( t_CurrentHighCardPlayerID == m_tMasterGameState.m_sPlayer2ID ) {
			m_tMasterGameState.m_sPlayer2CurrentTrickScore += t_score;
			m_tMasterGameState.m_byTrickLeadPlayer = (byte)m_tMasterGameState.m_sPlayer2ID;
			m_tMasterGameState.m_byCurrentPlayerTurn = (byte)m_tMasterGameState.m_sPlayer2ID;

			if ( m_tMasterGameState.m_sPlayer2CurrentTrickScore == Constant.MAX_SCORE_VALUE )
			{
				m_tMasterGameState.m_sPlayer1CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer3CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer4CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer2CurrentTrickScore = 0;
			}
		} else if ( t_CurrentHighCardPlayerID == m_tMasterGameState.m_sPlayer3ID ) {
			m_tMasterGameState.m_sPlayer3CurrentTrickScore += t_score;
			m_tMasterGameState.m_byTrickLeadPlayer = (byte)m_tMasterGameState.m_sPlayer3ID;
			m_tMasterGameState.m_byCurrentPlayerTurn = (byte)m_tMasterGameState.m_sPlayer3ID;

			if ( m_tMasterGameState.m_sPlayer3CurrentTrickScore == Constant.MAX_SCORE_VALUE ) {
				m_tMasterGameState.m_sPlayer1CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer2CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer4CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer3CurrentTrickScore = 0;
			}
		} else if ( t_CurrentHighCardPlayerID == m_tMasterGameState.m_sPlayer4ID ) {
			m_tMasterGameState.m_sPlayer4CurrentTrickScore += t_score;
			m_tMasterGameState.m_byTrickLeadPlayer = (byte)m_tMasterGameState.m_sPlayer4ID;
			m_tMasterGameState.m_byCurrentPlayerTurn = (byte)m_tMasterGameState.m_sPlayer4ID;

			if ( m_tMasterGameState.m_sPlayer4CurrentTrickScore == Constant.MAX_SCORE_VALUE ) {
				m_tMasterGameState.m_sPlayer1CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer2CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer3CurrentTrickScore = Constant.MAX_SCORE_VALUE;
				m_tMasterGameState.m_sPlayer4CurrentTrickScore = 0;
			}
		}

	}
	private void MoveTrickPointsToTotalScoreForPlayer()
	{
		m_tMasterGameState.m_sPlayer1TotalScore += m_tMasterGameState.m_sPlayer1CurrentTrickScore;
		m_tMasterGameState.m_sPlayer2TotalScore += m_tMasterGameState.m_sPlayer2CurrentTrickScore;
		m_tMasterGameState.m_sPlayer3TotalScore += m_tMasterGameState.m_sPlayer3CurrentTrickScore;
		m_tMasterGameState.m_sPlayer4TotalScore += m_tMasterGameState.m_sPlayer4CurrentTrickScore;
	}

	private void ResetTrick( ) {
		// m_tMasterGameState.m_tListCurrentTrickCards.clear();
		m_tMasterGameState.m_sTrickPlayerTurnCount = 1;
		m_tMasterGameState.m_eCurrentTrickSuit = null;
		
		m_tMasterGameState.m_tPlayer1TrickCard.m_tCardValue = null;
		m_tMasterGameState.m_tPlayer2TrickCard.m_tCardValue = null;
		m_tMasterGameState.m_tPlayer3TrickCard.m_tCardValue = null;
		m_tMasterGameState.m_tPlayer4TrickCard.m_tCardValue = null;
		m_tMasterGameState.m_tPlayer1TrickCard.m_tSuitValue = null;
		m_tMasterGameState.m_tPlayer2TrickCard.m_tSuitValue = null;
		m_tMasterGameState.m_tPlayer3TrickCard.m_tSuitValue = null;
		m_tMasterGameState.m_tPlayer4TrickCard.m_tSuitValue = null;
		
		m_tMasterGameState.m_sPlayer1CurrentTrickScore = 0;
		m_tMasterGameState.m_sPlayer2CurrentTrickScore = 0;
		m_tMasterGameState.m_sPlayer3CurrentTrickScore = 0;
		m_tMasterGameState.m_sPlayer4CurrentTrickScore = 0;
		
	}
	private boolean AnyPlayerAtMaxPoints(){
		if( m_tMasterGameState.m_sPlayer1TotalScore >= Constant.MAX_PLAYER_SCORE )
			return true;
		else if(m_tMasterGameState.m_sPlayer2TotalScore >= Constant.MAX_PLAYER_SCORE )
			return true;
		else if( m_tMasterGameState.m_sPlayer3TotalScore >= Constant.MAX_PLAYER_SCORE )
			return true;
		else if( m_tMasterGameState.m_sPlayer4TotalScore >= Constant.MAX_PLAYER_SCORE )
			return true;
		else
			return false;
	}
	private void StartNewRound()
	{	
		m_tMasterGameState.m_bIsHeartsBroken = false;
		m_tMasterGameState.m_byTrickLeadPlayer = 0;
		m_tMasterGameState.m_eCurrentGameState = GameState.DEALING;
		m_tMasterGameState.m_eCurrentTrickSuit = null;
		m_tMasterGameState.m_byCurrentPlayerTurn = Constant.PLAYER_ONE;
		m_tMasterGameState.m_tListGameDeck.clear();
		m_tMasterGameState.m_tListCurrentTrickCards.clear();
		
		
		//clear player 1 hand info just incase there is something left over
		m_tMasterGameState.m_tListPlayer1Hand.clear();
		m_tMasterGameState.m_tListPlayer1CardsWon.clear();
		m_tMasterGameState.m_tListPlayer1PassCards.clear();
		m_tMasterGameState.m_tPlayer1TrickCard.m_tCardValue = null;
		m_tMasterGameState.m_tPlayer1TrickCard.m_tSuitValue = null;
			
		//player 2
		m_tMasterGameState.m_tListPlayer2Hand.clear();
		m_tMasterGameState.m_tListPlayer2CardsWon.clear();
		m_tMasterGameState.m_tListPlayer2PassCards.clear();
		m_tMasterGameState.m_tPlayer2TrickCard.m_tCardValue = null;
		m_tMasterGameState.m_tPlayer2TrickCard.m_tSuitValue = null;
		
		// then player 3
		m_tMasterGameState.m_tListPlayer3Hand.clear();
		m_tMasterGameState.m_tListPlayer3CardsWon.clear();
		m_tMasterGameState.m_tListPlayer3PassCards.clear();
		m_tMasterGameState.m_tPlayer3TrickCard.m_tCardValue = null;
		m_tMasterGameState.m_tPlayer3TrickCard.m_tSuitValue = null;
		
		//finnally player 4
		m_tMasterGameState.m_tListPlayer4Hand.clear();
		m_tMasterGameState.m_tListPlayer4CardsWon.clear();
		m_tMasterGameState.m_tListPlayer4PassCards.clear();
		m_tMasterGameState.m_tPlayer4TrickCard.m_tCardValue = null;
		m_tMasterGameState.m_tPlayer4TrickCard.m_tSuitValue = null;
		
		m_tMasterGameState.m_sRoundCount++;
		m_tMasterGameState.m_sTrickCount = 1;
		
		GenerateGameDeck();		
		SycnronizePlayerStates();

	}
	private boolean HandlePassCardsState()
	{

		//////////////////////////////////////////////////////////////
		if(GameMode.Debug)
			DebugOutput( "Lets pass some cards" );
		//////////////////////////////////////////////////////////////
		SycnronizePlayerStates();
		if ( m_tMasterGameState.m_eNextPassDirection == PassDirection.NONE ) {
			if( GameMode.Debug )
				DebugOutput( "Pass Direction None " );
			m_tMasterGameState.m_eCurrentGameState = GameState.PLAYING;
			m_tMasterGameState.m_byCurrentPlayerTurn = Constant.PLAYER_NONE;
			m_tMasterGameState.m_eNextPassDirection = PassDirection.LEFT;
			return true;

		} else {
			do
			{
			// Temp vars for code readability while passing cards.
			Card t_card = new Card();
			List<Card> t_currentPlayerHand = new ArrayList<Card>();
			List<Card> t_currentPlayerPassCards = new ArrayList<Card>();
			AIPlayer t_player = new AIPlayer();

			
			Message t_msg = new Message();
			t_msg = m_tMessageHandler.GetMessages();
			t_card = ParseMessageToCard( t_msg );
			if( t_msg == null)
				return true;
			// set all the temp vars to current Players.
			if ( t_msg.m_iPlayerID == Constant.PLAYER_ONE ) {

				t_player = m_tPlayer1;
				t_currentPlayerPassCards = m_tMasterGameState.m_tListPlayer1PassCards;
				t_currentPlayerHand = m_tMasterGameState.m_tListPlayer1Hand;

			} else if ( t_msg.m_iPlayerID  == Constant.PLAYER_TWO ) {

				t_player = m_tPlayer2;
				t_currentPlayerPassCards = m_tMasterGameState.m_tListPlayer2PassCards;
				t_currentPlayerHand = m_tMasterGameState.m_tListPlayer2Hand;

			} else if ( t_msg.m_iPlayerID == Constant.PLAYER_THREE ) {

				t_player = m_tPlayer3;
				t_currentPlayerPassCards = m_tMasterGameState.m_tListPlayer3PassCards;
				t_currentPlayerHand = m_tMasterGameState.m_tListPlayer3Hand;

			} else if ( t_msg.m_iPlayerID == Constant.PLAYER_FOUR ) {

				t_player = m_tPlayer4;
				t_currentPlayerPassCards = m_tMasterGameState.m_tListPlayer4PassCards;
				t_currentPlayerHand = m_tMasterGameState.m_tListPlayer4Hand;
			}

				if ( !CheckPlayerHasCard( t_player.m_sAIPlayerID , t_card ) )
					return true; //i--; 
				else {
					t_currentPlayerPassCards.add( t_card );
					t_currentPlayerHand.remove( t_card );
				}
			}while( m_tMessageHandler.QueueHasDataToRead() );
				
				if(AllPlayersHavePassedCards() )
				{
					m_tMasterGameState.m_byCurrentPlayerTurn = Constant.PLAYER_NONE;
				}


			// Since we are here we need to actually pass the cards to the
			// other players...
			// The reason I added this step was the fact that doing it the
			// way I was doing
			// it added the passed cards to the next player immedietly so on
			// there turn to
			// pick passed cards those cards would now be in there hand. so
			// i came up with this way
			// now I will go through and actually pass the cards to each
			// player now that everyone has
			// picked there cards.
			if ( m_tMasterGameState.m_byCurrentPlayerTurn == Constant.PLAYER_NONE ) {

				/*
				 * HARDCODED FOR NOW... GAME TABLE Player 3 Player 2 Player
				 * 4 Player 1
				 */
				switch ( m_tMasterGameState.m_eNextPassDirection ) {
				case LEFT: {
					// Pass player4's cards to player1
					m_tMasterGameState.m_tListPlayer1Hand
							.addAll( m_tMasterGameState.m_tListPlayer4PassCards );
					m_tMasterGameState.m_tListPlayer4PassCards.clear();

					// Pass Player1's cards to player2
					m_tMasterGameState.m_tListPlayer2Hand
							.addAll( m_tMasterGameState.m_tListPlayer1PassCards );
					m_tMasterGameState.m_tListPlayer1PassCards.clear();

					// Pass Player2's cards to player3
					m_tMasterGameState.m_tListPlayer3Hand
							.addAll( m_tMasterGameState.m_tListPlayer2PassCards );
					m_tMasterGameState.m_tListPlayer2PassCards.clear();

					// Pass Player3's cards to player4
					m_tMasterGameState.m_tListPlayer4Hand
							.addAll( m_tMasterGameState.m_tListPlayer3PassCards );
					m_tMasterGameState.m_tListPlayer3PassCards.clear();

					System.out.println( "Passed cards to the left" );
					m_tMasterGameState.m_eNextPassDirection = PassDirection.RIGHT;
					break;
				}
				case RIGHT: {
					// Pass Player2's cards to Player1
					m_tMasterGameState.m_tListPlayer1Hand
							.addAll( m_tMasterGameState.m_tListPlayer2PassCards );
					m_tMasterGameState.m_tListPlayer2PassCards.clear();

					// pass Player3's Cards to Player2
					m_tMasterGameState.m_tListPlayer2Hand
							.addAll( m_tMasterGameState.m_tListPlayer3PassCards );
					m_tMasterGameState.m_tListPlayer3PassCards.clear();

					// pass Player4's cards to Player3
					m_tMasterGameState.m_tListPlayer3Hand
							.addAll( m_tMasterGameState.m_tListPlayer4PassCards );
					m_tMasterGameState.m_tListPlayer4PassCards.clear();

					// pass Player1's cards to Player4
					m_tMasterGameState.m_tListPlayer4Hand
							.addAll( m_tMasterGameState.m_tListPlayer1PassCards );
					m_tMasterGameState.m_tListPlayer1PassCards.clear();

					System.out.println( "Passed cards to the Right" );
					m_tMasterGameState.m_eNextPassDirection = PassDirection.ACROSS;
					break;

				}
				case ACROSS: {
					// Pass Player3's cards to player1
					m_tMasterGameState.m_tListPlayer1Hand
							.addAll( m_tMasterGameState.m_tListPlayer3PassCards );
					m_tMasterGameState.m_tListPlayer3PassCards.clear();

					// Pass Player4's cards to Player2
					m_tMasterGameState.m_tListPlayer2Hand
							.addAll( m_tMasterGameState.m_tListPlayer4PassCards );
					m_tMasterGameState.m_tListPlayer4PassCards.clear();

					// Pass Player1's cards to Player3
					m_tMasterGameState.m_tListPlayer3Hand
							.addAll( m_tMasterGameState.m_tListPlayer1PassCards );
					m_tMasterGameState.m_tListPlayer1PassCards.clear();

					// Pass Player2's cards to Player4
					m_tMasterGameState.m_tListPlayer4Hand
							.addAll( m_tMasterGameState.m_tListPlayer2PassCards );
					m_tMasterGameState.m_tListPlayer2PassCards.clear();

					System.out.println( "Passed cards across the table " );
					m_tMasterGameState.m_eNextPassDirection = PassDirection.NONE;
					break;
				}
				}
				// everything in this state is done so lets move on.
				m_tMasterGameState.m_byCurrentPlayerTurn = Constant.PLAYER_NONE;
				m_tMasterGameState.m_eCurrentGameState = GameState.PLAYING;
				
				return true;
			}
		}
		return true;

	}
	private boolean HandlePlayingState()
	{
		int t_RoundStartPlayer = -1;
		// If PlayersHave all cards
		// Then its the start of a round
		// Need to find who has two of clubs for start of game
		if ( m_tMasterGameState.m_sTrickCount == Constant.FIRST_TRICK_OF_ROUND
				&& m_tMasterGameState.m_byCurrentPlayerTurn == Constant.PLAYER_NONE )
		{
			if( GameMode.Debug )
				System.out.println( "It is the first hand of the round finding player with 2 of" + " clubs" );

			t_RoundStartPlayer = FindPlayerWithTwoOfClubs();
			if ( t_RoundStartPlayer != -1 ) {
				if( GameMode.Debug )
					System.out.println( "Player" + t_RoundStartPlayer + " is going to start the round" );
				m_tMasterGameState.m_byCurrentPlayerTurn = (byte)t_RoundStartPlayer;
				m_tMasterGameState.m_byTrickLeadPlayer = (byte)t_RoundStartPlayer;
				m_tMasterGameState.m_eCurrentTrickSuit = SuitType.CLUBS;
				
			}

		}
		// if players have > 0 cards in there hand. then a another trick
		// should be played.
		if ( !PlayersHaveZeroCards() ) {

			do
			{
				//if( !AllPlayersHaveSameSizeHand() )
					//System.out.println( "STOP HERE THE DECKS ARE CORRUPTED" );
				Message t_msg = new Message();
				t_msg = m_tMessageHandler.GetMessages();

				if( GameMode.Debug && m_tMasterGameState.m_sTrickPlayerTurnCount == Constant.FIRST_PLAYER_OF_TRICK )
				{
					System.out.println( "                           Entering PLAYING GameState" );
					DebugOutput( "                              Round:" + m_tMasterGameState.m_sRoundCount + " Trick:" + m_tMasterGameState.m_sTrickCount );
					PrintPlayerDeckSizeForEachPlayer();
				}

				AIPlayer t_CurrentPlayer = new AIPlayer();
				Card t_PlayedCard = new Card();
				boolean t_IsCardValid = false;
				List<Card> t_WrongChoiceHand = new ArrayList<Card>();

				if ( m_tMasterGameState.m_byCurrentPlayerTurn == Constant.PLAYER_ONE && t_msg.m_iPlayerID == Constant.PLAYER_ONE )
				{
					t_CurrentPlayer = m_tPlayer1;
					t_WrongChoiceHand = m_tMasterGameState.m_tListPlayer1UsedCards;
				}
				else if ( m_tMasterGameState.m_byCurrentPlayerTurn == Constant.PLAYER_TWO && t_msg.m_iPlayerID == Constant.PLAYER_TWO )
				{
					t_CurrentPlayer = m_tPlayer2;
					t_WrongChoiceHand = m_tMasterGameState.m_tListPlayer2UsedCards;
				}
				else if ( m_tMasterGameState.m_byCurrentPlayerTurn == Constant.PLAYER_THREE && t_msg.m_iPlayerID == Constant.PLAYER_THREE )
				{
					t_CurrentPlayer = m_tPlayer3;
					t_WrongChoiceHand = m_tMasterGameState.m_tListPlayer3UsedCards;
				}
				else if ( m_tMasterGameState.m_byCurrentPlayerTurn == Constant.PLAYER_FOUR && t_msg.m_iPlayerID == Constant.PLAYER_FOUR )
				{
					t_CurrentPlayer = m_tPlayer4;
					t_WrongChoiceHand = m_tMasterGameState.m_tListPlayer4UsedCards;
				}
				else
					return true;

				if( GameMode.Debug )
					System.out.println( "Player" + t_CurrentPlayer.m_sAIPlayerID + " is picking a card" );

				t_PlayedCard = ParseMessageToCard( t_msg );
			
				t_IsCardValid = CheckTrickRulesOnCard(
						t_CurrentPlayer.m_sAIPlayerID , t_PlayedCard );

				if ( t_IsCardValid ) {
					if (m_tMasterGameState.m_sTrickPlayerTurnCount == 
							Constant.FIRST_TURN_OF_TRICK ) {
						m_tMasterGameState.m_eCurrentTrickSuit = t_PlayedCard.m_tSuitValue;
					}
					SetNextPlayerTurn();
					AddCardToPlayerTrickCard( t_CurrentPlayer.m_sAIPlayerID , t_PlayedCard );
					RemoveCardFromPlayer( t_CurrentPlayer.m_sAIPlayerID , t_PlayedCard );

					if ( !t_WrongChoiceHand.isEmpty() ) {
						AddCardsToPlayerDeck(
								t_CurrentPlayer.m_sAIPlayerID ,
								t_WrongChoiceHand );
						t_WrongChoiceHand.clear();
					}
					if( GameMode.Debug )
					{
						DebugOutput( "CardPlayed:" + t_PlayedCard.m_tCardValue + " : " + t_PlayedCard.m_tSuitValue );
						DebugOutput( "By Player:" + t_CurrentPlayer.m_sAIPlayerID );
					}

				} else {
					RemoveCardFromPlayer( t_CurrentPlayer.m_sAIPlayerID ,
							t_PlayedCard );
					t_WrongChoiceHand.add( t_PlayedCard );
					return true;
				}
				SycnronizePlayerStates();
				
			}	while( m_tMessageHandler.QueueHasDataToRead() );

			// I feel like this is the best place to check if a trick has
			// completed.
			// So lets check if a trick has completed.
			if ( AllPlayersHavePlayedTrick() ) {
				if( GameMode.Debug )
					System.out.println( "               We are now calculating the points for this trick" );
				short t_TrickScore = CalculateTrickPointValue();
				FindTrickWinnerAndAssignPoints( t_TrickScore );
				MoveTrickPointsToTotalScoreForPlayer();
				ResetTrick();

				m_tMasterGameState.m_sTrickCount++;
				if(GameMode.Debug )
				{
					DebugOutput( "   Player1 Score:"
							+ m_tMasterGameState.m_sPlayer1TotalScore
							+ " - Player2 Score:"
							+ m_tMasterGameState.m_sPlayer2TotalScore
							+ " - Player3 Score:"
							+ m_tMasterGameState.m_sPlayer3TotalScore
							+ " - Player4 Score:"
							+ m_tMasterGameState.m_sPlayer4TotalScore );
				}
			}
			return true;
		}
		// If All Players have 0 cards then the round is over
		if ( PlayersHaveZeroCards() ) {				
			if( AnyPlayerAtMaxPoints() )
			{
				if( GameMode.Debug )
					System.out.println( "Game Over Good bye" );
				m_tMasterGameState.m_eCurrentGameState = GameState.ENDING;
				return false;
			}
			else{
				m_tMasterGameState.m_eCurrentGameState = GameState.STARTING;
				//StartNewRound();
				return true;
			}

		}
		return false;
	}
	
	private Card ParseMessageToCard( Message t_msg )
	{
		Card t_Failcard = new Card();
		t_Failcard.m_iCardID = -1;
		t_Failcard.m_tCardValue = null;
		t_Failcard.m_tSuitValue = null;
		
		if( t_msg.m_iPlayerID == Constant.PLAYER_ONE )
		{
			for( Card c : m_tMasterGameState.m_tListPlayer1Hand )
			{
				if( t_msg.m_iCardID == c.m_iCardID )
					return c;
			}
			
		}
		else if( t_msg.m_iPlayerID == Constant.PLAYER_TWO )
		{
			for( Card c : m_tMasterGameState.m_tListPlayer2Hand )
			{
				if( t_msg.m_iCardID == c.m_iCardID )
					return c;
			}
		}
		else if( t_msg.m_iPlayerID == Constant.PLAYER_THREE )
		{
			for( Card c : m_tMasterGameState.m_tListPlayer3Hand )
			{
				if( t_msg.m_iCardID == c.m_iCardID )
					return c;
			}
		}
		else if( t_msg.m_iPlayerID == Constant.PLAYER_FOUR )
		{
			for( Card c : m_tMasterGameState.m_tListPlayer4Hand )
			{
				if( t_msg.m_iCardID == c.m_iCardID )
					return c;
			}
		}
		else 
			return t_Failcard;
		
		
		return t_Failcard;
	}
	
	private boolean MessageIsFromCurrentPlayer( Message t_msg )
	{
		if( t_msg.m_iPlayerID == m_tMasterGameState.m_byCurrentPlayerTurn )
			return true;
		else
			return false;
	}
	/*
	 *         DISPLAY FUNCTIONS FOR DEBUG OR LOGGING... 
	 */
	private void DisplayConsoleDebugScreen()
	{
		FileWriter dst;
		String debugoutput =  "                               HEARTS"
		 + "/n Player State info: PLAYER1   --   Player2   --   Player3   --   Player4" 
		 + "/n ID:                  " + m_tMasterGameState.m_sPlayer1ID + "               " 
				+ m_tMasterGameState.m_sPlayer2ID + "              " + m_tMasterGameState.m_sPlayer3ID +
				"             " + m_tMasterGameState.m_sPlayer4ID 
		 + "/n Trick Score:         " + m_tMasterGameState.m_sPlayer2CurrentTrickScore + "              " +
				m_tMasterGameState.m_sPlayer2CurrentTrickScore + "               " + m_tMasterGameState.m_sPlayer3CurrentTrickScore + 
				"             " + m_tMasterGameState.m_sPlayer4CurrentTrickScore;
	}


	private void DebugOutput(  String t_message ) {
		System.out.println( t_message );
	}

	// STUPID PRINT FUNCTIONS SO I DONT CLUTTER UP MY GAME CODE TOO BAD WITH
	// LONG ASS PRINT STATEMENTS
	private void PrintPlayerDeckSizeForEachPlayer( ) {
		System.out.println( "                DeckSize- Player1:"
				+ m_tMasterGameState.m_tListPlayer1Hand.size() + " Player2:"
				+ m_tMasterGameState.m_tListPlayer2Hand.size() + " Player3:"
				+ m_tMasterGameState.m_tListPlayer3Hand.size() + " Player4:"
				+ m_tMasterGameState.m_tListPlayer4Hand.size() );
	}

}