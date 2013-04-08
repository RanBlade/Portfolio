package ranblade.hearts;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;
import java.util.Queue;

import ranblade.game.*;

public final class MessageDirector {
	private static Queue<Message> m_tMessageQueue = new LinkedList<Message>();

	
	private static Map< Short , Queue<Message>> m_mapActiveHandlers = new HashMap<Short , Queue<Message>>();
	private static short m_sMessageIDKey = 0;

	
	public static short m_shMainServerHandlerID = Constant.NULL;
		
	public static short RegisterHandler( HandlerType t_type, short t_id , Queue<Message> t_queue )
	{
		if(t_type == HandlerType.SERVER && m_shMainServerHandlerID == -1 )
		{

			m_shMainServerHandlerID = m_sMessageIDKey;
			m_mapActiveHandlers.put(t_id , t_queue );
			return Constant.GENERAL_SUCCESS;

		}
		else if( t_type == HandlerType.SERVER && m_mapActiveHandlers.containsKey(HandlerType.SERVER))
		{
			return ErrorCodes.ERROR_SERVER_ALREADY_DEFINED; //client/server should check fo this error value
		}
		else if( t_type == HandlerType.CLIENT )
		{			
			m_mapActiveHandlers.put(t_id , t_queue );
			return Constant.GENERAL_SUCCESS;
		}
		else
		{
			return ErrorCodes.ERROR_CANNOT_REGISTER_HANDLE; //Client/server should check for this error value
		}
	}
	//this is temporary to hopefully simplify the interface.
	public static void PushMessage( Message t_message )
	{
		m_tMessageQueue.add(t_message);
	}
	public static void Update()
	{
		if(!m_tMessageQueue.isEmpty())
		{
			while( !m_tMessageQueue.isEmpty() )
			{
				Message t_msg = new Message();
				Queue<Message> t_queue = new LinkedList<Message>();
				t_msg = m_tMessageQueue.poll();
				
				if(t_msg.m_iDestinationID == m_shMainServerHandlerID )
				{
					t_queue = m_mapActiveHandlers.get( m_shMainServerHandlerID );
					t_queue.add(t_msg);
					m_mapActiveHandlers.put(m_shMainServerHandlerID , t_queue );
				}
				else {
					for( Map.Entry<Short, Queue<Message>> entry : m_mapActiveHandlers.entrySet() )
					{
						if( t_msg.m_iDestinationID == entry.getKey() )
						{
							Queue<Message> t_queue2 = new LinkedList<Message>();
							t_queue2 = entry.getValue();
							t_queue2.add(t_msg);
							
							m_mapActiveHandlers.put(entry.getKey(), t_queue2 );
							
						}
					}
				}
				
			}
			
		}
		else
		{
			return;
		}
	}
}
