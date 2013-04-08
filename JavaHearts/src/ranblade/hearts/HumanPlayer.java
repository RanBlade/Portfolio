package ranblade.hearts;

import ranblade.game.*;
import java.util.*;


public class HumanPlayer {
	
	public HeartsState m_tMyGameState;
	
	public short m_sMyPlayerID;
	public short m_sMyTotalScore;
	public String m_stringMyName;
	public List<Card> m_tListMyHand;
	public List<Card> m_tListMyPassedCards;
	public MessageHandler m_tMessageHandler;
		
		public HumanPlayer(){
			m_sMyPlayerID = 0;
			m_tMyGameState = new HeartsState();
			m_tListMyHand = new ArrayList<Card>();
			m_tListMyPassedCards = new ArrayList<Card>();
			
			SyncDeckAndScoreToGameState();
			
			m_tMessageHandler = new MessageHandler( HandlerType.CLIENT , m_sMyPlayerID );
		}
		
		public void PassCards()
		{
	
		}
		public void PlayCard()
		{
			
		}
		public void SelectCard()
		{
			
		}
		public void Update()
		{
			SyncDeckAndScoreToGameState();
		}
		
		private void SyncDeckAndScoreToGameState()
		{
			if( m_sMyPlayerID == Constant.PLAYER_ONE )
			{
				//for( Card c : m_tMyGameState.m_tListPlayer1Hand)
				//{
				//	m_tListMyHand.add( c );
				//}
				m_sMyTotalScore = m_tMyGameState.m_sPlayer1TotalScore;
				m_tListMyHand    = m_tMyGameState.m_tListPlayer1Hand;
				m_tListMyPassedCards = m_tMyGameState.m_tListPlayer1PassCards;
			}
			if( m_sMyPlayerID == Constant.PLAYER_TWO )
			{
				m_sMyTotalScore = m_tMyGameState.m_sPlayer2TotalScore;
				m_tListMyHand    = m_tMyGameState.m_tListPlayer2Hand;
				m_tListMyPassedCards = m_tMyGameState.m_tListPlayer2PassCards;
			}
			if( m_sMyPlayerID == Constant.PLAYER_THREE )
			{
				m_sMyTotalScore = m_tMyGameState.m_sPlayer3TotalScore;
				m_tListMyHand    = m_tMyGameState.m_tListPlayer3Hand;
				m_tListMyPassedCards = m_tMyGameState.m_tListPlayer3PassCards;
			}
			if( m_sMyPlayerID == Constant.PLAYER_FOUR )
			{
				m_sMyTotalScore = m_tMyGameState.m_sPlayer4TotalScore;
				m_tListMyHand    = m_tMyGameState.m_tListPlayer4Hand;
				m_tListMyPassedCards = m_tMyGameState.m_tListPlayer4PassCards;
			}
		}
		
		
		private class InputReader
		{
			private Scanner m_in = new Scanner( System.in );
			
			public InputReader()
			{
				
			}
			
			public void ReadInputer( String t_InputString )
			{
				t_InputString = m_in.nextLine();
			}
		}
	
		
	}
