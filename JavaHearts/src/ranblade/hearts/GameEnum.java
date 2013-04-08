package ranblade.hearts;

public enum GameEnum {
	// PLAYER1 (1) ,
	// PLAYER2 (2) ,
	// PLAYER3 (3) ,
	// PLAYER4 (4) ,
	PASS_CARD_COUNT( 3 ) ,
	START_ROUND_CARD_COUNT( 13 ) ,
	MAX_TRICK_CARDS( 4 ) ,
	MAX_PLAYED_TRICK(13 ) ,
	FIRST_TRICK_OF_ROUND( 1 ),
	HEART_POINT_VALUE( 1 ),
	QUEEN_SPAIDS_POINT_VALUE( 13 ),
	MAX_SCORE_VALUE( QUEEN_SPAIDS_POINT_VALUE.Value()
					+ (HEART_POINT_VALUE.Value() * MAX_PLAYED_TRICK.Value())) ,
	FIRST_TURN_OF_TRICK(1),
	MAX_PLAYER_SCORE( 100 );
	

	private int m_iValues;

	GameEnum( int t_values ) {
		this.m_iValues = t_values;
	}

	public int Value( ) {
		return m_iValues;
	}
}
