#ifndef _USER_H_
#define _USER_H_


#include "connection.h"
#include "support/support.h"
#include "useraccount.h"
#include "eventagent.h"
#include "ServerWriterAdapter.h"


class UserAgent : public Connection
{
    public:
    //////////////////////////////////////////////////////////////////////////////////////////////
        UserAgent(EventAgent *t_agent , SOCKET t_sock , const Socket_Address &t_addr ) : Connection(t_sock , t_addr)
        {
	    m_EventAgent = t_agent;
	    m_EventAgent->SetConnection(this);
            m_allConnections.insert(this);
	    
        }
        ~UserAgent()
        {
          m_allConnections.erase(this);
	  LOGINFO("Done removing Connection and UserAgent\n");

        }
        void pre_trash()
	{
	  LOGINFO("Attemption to save all accounts\n");
	  SaveAllAccounts();
	  LOGINFO("Attempting to disconnect All Users\n");
	  DisconnectAllAccounts();
	  
	  m_EventAgent->RemoveConnection(this);
	  //add_to_trash();
	}
    //////////////////////////////////////////////////////////////////////////////////////////////
	void Process()
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
	    for(std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.begin(); ii != ConnectedUserAccounts.end(); ii++)
	    {
	      (*ii)->m_timeoutState.Process();
	      if((*ii)->m_timeoutState._flush_time)
	      {
		BinaryMessage t_msg(4096);
		ServerWriterAdapter t_writer(t_msg);
		t_writer.AddInt2u(PING_USER_STILL_CONNECTED);
		//t_writer.AddInt2u((*ii)->m_pingchallenge);
		LOGINFO("ClientID:%i \n" , (*ii)->m_clientID);
		t_writer.AddInt2u((*ii)->m_clientID);
		QueueForWrite(t_msg);
	      }
	    }
	  }

	 
	// LOGINFO("TESTING FOR SEGFAULT: Leaving Connection::Process\n");
	}
        void ProcessMessageFromClient( BinaryMessage t_msg );
		void ProcessUserLogin(ServerReaderAdapter& , BinaryMessage&);

    protected:
    private:
      static UINT16 m_idSeed;
      EventAgent* m_EventAgent;
      
      static std::set<UserAgent*> m_allConnections;    
};

class User
{
private:
  std::string m_userPasswrd;
  std::string m_userName;
  std::string m_userBalance;
  
};
#endif // _USER_H_
