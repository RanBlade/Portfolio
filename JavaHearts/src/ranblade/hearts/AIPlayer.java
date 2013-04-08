package ranblade.hearts;

import ranblade.game.*;
import java.util.*;

public class AIPlayer {

	public HeartsState m_tAIPlayerGameState;	
	public short m_sAIPlayerID;
	public short m_sAIPlayerTotalScore;
	public String m_stringAIPlayerName;
	public List<Card> m_tListAIPlayerHand;
	public List<Card> m_tListAIPlayerPassedCards;
	public MessageHandler m_tAIPlayerMessageHandler;

	public AIPlayer() {
		m_sAIPlayerID = 0;
		m_tAIPlayerGameState = new HeartsState();
		m_tListAIPlayerHand = new ArrayList<Card>();
		m_tListAIPlayerPassedCards = new ArrayList<Card>();
		
		SyncDeckAndScoreToGameState();
		
		m_tAIPlayerMessageHandler = new MessageHandler(HandlerType.CLIENT , m_sAIPlayerID);

	}	

	public void Update( ) 
	{
		SyncDeckAndScoreToGameState();
		
		if( m_tAIPlayerGameState.m_byCurrentPlayerTurn == m_sAIPlayerID )
		{
			if( m_tAIPlayerGameState.m_eCurrentGameState == GameState.PLAYING )
			{
				PlayCard();

			}
		}
		if( m_tAIPlayerGameState.m_eCurrentGameState == GameState.PASSINGCARDS && m_tListAIPlayerPassedCards.size() != Constant.PASS_CARD_COUNT )
		{
				//for( int i = 0; i < GameConstant.PASS_CARD_COUNT; i++)
			//	{
					PlayCard();
			//	}
		}
		SyncDeckAndScoreToGameState();
	}
	
	private Card PlayCard( ) {
		Random t_generator = new Random();
		Card t_card = new Card();
		
		if ( m_tListAIPlayerHand.size() > 0 )
			t_card = m_tListAIPlayerHand.get( t_generator.nextInt( m_tListAIPlayerHand.size() ) );

		Message t_msg = new Message( MessageType.PLAY_CARD , MessageDirector.m_shMainServerHandlerID, 
				m_tAIPlayerMessageHandler.m_shHandlerID, t_card.m_iCardID );
		MessageDirector.PushMessage( t_msg );
		

		
		return t_card;
	}
	
	private void SyncDeckAndScoreToGameState()
	{
		if( m_sAIPlayerID == Constant.PLAYER_ONE )
		{
			//for( Card c : m_tMyGameState.m_tListPlayer1Hand)
			//{
			//	m_tListMyHand.add( c );
			//}
			m_sAIPlayerTotalScore = m_tAIPlayerGameState.m_sPlayer1TotalScore;
			m_tListAIPlayerHand    = m_tAIPlayerGameState.m_tListPlayer1Hand;
			m_tListAIPlayerPassedCards = m_tAIPlayerGameState.m_tListPlayer1PassCards;
		}
		if( m_sAIPlayerID == Constant.PLAYER_TWO )
		{
			m_sAIPlayerTotalScore = m_tAIPlayerGameState.m_sPlayer2TotalScore;
			m_tListAIPlayerHand    = m_tAIPlayerGameState.m_tListPlayer2Hand;
			m_tListAIPlayerPassedCards = m_tAIPlayerGameState.m_tListPlayer2PassCards;
		}
		if( m_sAIPlayerID == Constant.PLAYER_THREE )
		{
			m_sAIPlayerTotalScore = m_tAIPlayerGameState.m_sPlayer3TotalScore;
			m_tListAIPlayerHand    = m_tAIPlayerGameState.m_tListPlayer3Hand;
			m_tListAIPlayerPassedCards = m_tAIPlayerGameState.m_tListPlayer3PassCards;
		}
		if( m_sAIPlayerID == Constant.PLAYER_FOUR )
		{
			m_sAIPlayerTotalScore = m_tAIPlayerGameState.m_sPlayer4TotalScore;
			m_tListAIPlayerHand    = m_tAIPlayerGameState.m_tListPlayer4Hand;
			m_tListAIPlayerPassedCards = m_tAIPlayerGameState.m_tListPlayer4PassCards;
		}
	}

	
}