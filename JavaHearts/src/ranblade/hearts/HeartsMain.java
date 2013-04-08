package ranblade.hearts;

//import java.io.*;


public class HeartsMain {

	/**
	 * @param args
	 */
	private static boolean m_bGameIsRunning = false;
	
	public static void main(String[] args) {
		System.out.println( " Application starting " );
		m_bGameIsRunning = true;
		
		
		AIPlayer m_tPlayer1 = new AIPlayer();
		AIPlayer m_tPlayer2 = new AIPlayer();
		AIPlayer m_tPlayer3 = new AIPlayer();
		AIPlayer m_tPlayer4 = new AIPlayer();
		
		SinglePlayerGameTable t_table = new SinglePlayerGameTable( m_tPlayer1 , m_tPlayer2, m_tPlayer3, m_tPlayer4);
		
		while( m_bGameIsRunning )
		{
			m_bGameIsRunning = t_table.Update();
			m_tPlayer1.Update();
			m_tPlayer2.Update();
			m_tPlayer3.Update();
			m_tPlayer4.Update();
		}

		
		
		
		System.out.println( " Application closing " );
	}

}

/* 
 * 
 * 
 *
 * */
 