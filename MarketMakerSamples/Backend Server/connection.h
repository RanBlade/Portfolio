#ifndef _CONNECTION_H_
#define _CONNECTION_H_

#include "support/support.h"
#include "process_state.h"
#include "log/log.h"
#include "socket/socket_tcp.h"
#include "network/bufferedreader.h"
#include "network/bufferedwriter.h"
#include "trash/trashcan.h"

class Connection : public trashable
{
public:
    typedef std::set<Connection*> ConnectionContainer;

    Connection(SOCKET t_sock , const Socket_Address t_addr) : m_socket(t_sock) , m_addr(t_addr) , m_reader(8192) , m_writer(false, 8192, 1024)
    {
        m_socket.SetNonBlocking();
        m_allConnections.insert(this);
        LOGINFO("New Connection %s \n" , m_addr.get_ip_port().c_str());
    }
    virtual ~Connection()
    {
        m_allConnections.erase(this);
        LOGINFO("Remove Connection %s \n" , m_addr.get_ip_port().c_str());
    }

    const Socket_TCP GetSock(){ return m_socket;}
    const Socket_Address GetAddr(){ return m_addr;}
    void pre_trash()
    {
        LOGINFO("Getting ready to delete client %s\n" , m_addr.get_ip_port().c_str());
    }
    
    virtual void Process()
    {
      
        BinaryMessage t_msg;
        int ans = 1;
	//LOGINFO("TESTING FOR SEGFAULT: Entering COnnection::Process\n");
        while( ans > 0 )
        {
            ans = m_reader.PumpMessageReaderLocal( t_msg, m_socket);

            if( ans < 0)
            {
                LOGINFO("Socket read error %s\n" , m_addr.get_ip_port().c_str());
                add_to_trash();
            }
            if( ans > 0 )
            {
	      LOGINFO("About to ProcessMessageFromClient\n");
                ProcessMessageFromClient( t_msg );
		//LOGINFO("TESTING FOR SEGFAULT: Leaving If(ans > 0)\n");
            }
           
        }
       // LOGINFO("TESTING FOR SEGFAULT: Leaving Connection::Process\n");
    }
    static void ProcessAllNetworkClients(ProcessState &t_state)
    {
        for(ConnectionContainer::iterator ii = m_allConnections.begin(); ii != m_allConnections.end(); ii++)
        {
	  //LOGINFO("ProcessAllNetworkClients Process Client\n");
            (*ii)->Process();
        }
        if(t_state._flush_time)
	{
	  for(ConnectionContainer::iterator ii = m_allConnections.begin(); ii != m_allConnections.end(); ii++)
	  {
	    //LOGINFO("ProcessAllNetworkClients Flush Client\n");
	    (*ii)->Flush();
	  }
	}
      //LOGINFO("TESTING FOR SEGFAULT: Leaving Connection::ProcessAllNetworkClients\n");
    }
    
    void QueueForWrite(BinaryMessage & msg)
    {
       int resp =  m_writer.AddMessage(msg,m_socket);
       if(resp < 0)
       {
            LOGINFO("Socket Error In Write %s\n", m_addr.get_ip_port().c_str());
            add_to_trash();
        }
    }
    void Flush()
    {
     // LOGINFO("Flushing Connection buffer");
        if(m_writer.FlushNoBlock(m_socket) < 0)
        {
            LOGINFO("Socket Error In Flush %s\n",m_addr.get_ip_port().c_str());
            add_to_trash();
        }
        
    }


    virtual void ProcessMessageFromClient( BinaryMessage t_msg ) = 0;

protected:
    Socket_TCP m_socket;
    Socket_Address m_addr;

    BufferedReader m_reader;
    BufferedWriter m_writer;
    static std::set< Connection* > m_allConnections;
};

#endif
