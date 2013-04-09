#ifndef _NETWORKMANAGER_H_
#define _NETWORKMANAGER_H_


#include "user.h"
#include "socket/socket_tcp_listen.h"
#include "process_state.h"

class NetworkManager
{
    typedef void(*NewConnectionProcessor)(SOCKET t_sock1, const Socket_Address &t_addr1);

public:
    /** Default constructor - dont use this right now */
    NetworkManager(){}
    //use this constructor please!
    NetworkManager(short t_port) : m_port(t_port) {}

    /** Default destructor */
    ~NetworkManager(){}

    //simple init function
    bool init(void)
    {
        Socket_Address t_addr;
        t_addr.set_any_IP(m_port);

        if(!m_listener.OpenForListen(t_addr, false , 1024))
        {
            m_listener.Close();
            LOGINFO("LISTEN/BIND ERROR");
        }
        else
        {
            m_listener.SetNonBlocking();
            LOGINFO("Now Listening on port:%i and socket set non blocking\n" , m_port);
        }

        return m_listener.Active();
    }

    //sorta a workhorse function. but only for the listener
    int ProcessServer(ProcessState &t_state , NewConnectionProcessor t_factory)
    {
        SOCKET m_sock;
        Socket_Address m_addr;

        while( m_listener.GetIncomingConnection(m_sock , m_addr))
        {
            t_factory( m_sock, m_addr);
        }
        return 0;
    }

protected:
    Socket_TCP_Listen m_listener;
    short             m_port;
private:
};


#endif // _NETWORKMANAGER_H_
