package ranblade.game;

public final class Constant {
	
	//general constants
	public final static byte NULL = -1;
	public final static byte GENERAL_SUCCESS = 0;
	
	//standard game constants
	public final static byte PASS_CARD_COUNT = 3;
	public final static byte START_ROUND_CARD_COUNT = 13;
	public final static byte MAX_TRICK_CARDS = 4;
	public final static byte MAX_PLAYED_TRICK = 13;
	public final static byte FIRST_TRICK_OF_ROUND = 1;
	public final static byte FIRST_PLAYER_OF_TRICK = 1;
	public final static byte HEART_POINT_VALUE = 1;
	public final static byte QUEEN_SPAIDS_POINT_VALUE = 13;
	public final static byte MAX_SCORE_VALUE = QUEEN_SPAIDS_POINT_VALUE + (HEART_POINT_VALUE * MAX_PLAYED_TRICK);
	public final static byte FIRST_TURN_OF_TRICK = 1;
	public final static byte MAX_PLAYER_SCORE = 100;
	
	//player constants
	public final static byte PLAYER_NONE = 0;
	public final static byte PLAYER_ONE = 1;
	public final static byte PLAYER_TWO = 2;
	public final static byte PLAYER_THREE = 3;
	public final static byte PLAYER_FOUR = 4;
	
	
}
