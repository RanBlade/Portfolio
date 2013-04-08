package ranblade.hearts;

import ranblade.game.*;
import java.util.*;

public class MessageHandler {
	public short m_shHandlerID;
	private Queue<Message> m_tMessageQueue = new LinkedList<Message>();
	
	MessageHandler(HandlerType t_type , short t_id )
	{
		m_shHandlerID = t_id;
		MessageDirector.RegisterHandler(t_type , m_shHandlerID , m_tMessageQueue );
	}
	
	public Message GetMessages()
	{
		Message t_msg = new Message();
		if( !m_tMessageQueue.isEmpty() )
		{
			t_msg = m_tMessageQueue.poll();
			return t_msg;
		}
		else{
			t_msg.m_iCardID = -2;
			t_msg.m_iPlayerID = -2;
			return t_msg;
		}
	
	}
	
	public boolean QueueHasDataToRead()
	{
		if( m_tMessageQueue.isEmpty() )
			return false;
		else
			return true;
	}
}
