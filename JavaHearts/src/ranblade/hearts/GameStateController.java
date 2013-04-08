package ranblade.hearts;


public class GameStateController {
	private static HeartsState m_tCurrentGameState;
	
	public void Init( HeartsState t_GameState)
	{
		m_tCurrentGameState = t_GameState;
	}
	//private helper functions
	//GetPlayerdeck(id)
	//GetPlayer
	
	
	//public fucntions
	public GameState GetCurrentGameState()
	{
		return m_tCurrentGameState.m_eCurrentGameState;
	}
	public void ChangeGameState(GameState t_state)
	{
		m_tCurrentGameState.m_eCurrentGameState = t_state;
	}
	
	public boolean IsHeartsBroken()
	{
		return m_tCurrentGameState.m_bIsHeartsBroken;
	}
	public void BreakHearts()
	{
		m_tCurrentGameState.m_bIsHeartsBroken = true;
	}
	public void ResetBrokenHeartState()
	{
		m_tCurrentGameState.m_bIsHeartsBroken = false;
	}
	
	public SuitType GetCurrentTrickSuit()
	{
		return m_tCurrentGameState.m_eCurrentTrickSuit;
	}
	public void ChangeCurrentTrickSuit(SuitType t_suit)
	{
		m_tCurrentGameState.m_eCurrentTrickSuit = t_suit;
	}
	//RemoveCardFromPlayer
	//AddCardToPlayerDeck
	//
}
