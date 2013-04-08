package ranblade.hearts;


class Card
{
	public int          m_iCardID;
	public CardType     m_tCardValue;
	public SuitType      m_tSuitValue;
	//public SceneNode m_tCardNode;

	public Card()
	{
		m_iCardID    = 0;
		m_tCardValue = null;
		m_tSuitValue = null;
		//m_tCardNode  = new SceneNode( null );
	}
}