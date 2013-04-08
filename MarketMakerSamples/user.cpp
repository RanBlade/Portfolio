#include "user.h"
#include "ServerReaderAdapter.h"
#include "ServerWriterAdapter.h"
#include "functioncodes.h"
#include "useraccount.h"
#include "dbmanager.h"

UINT16 UserAgent::m_idSeed = 0;
std::set<UserAgent*> UserAgent::m_allConnections;

void UserAgent::ProcessMessageFromClient(BinaryMessage t_msg)
{
	ServerReaderAdapter t_reader(t_msg);
	int t_funcCode = t_reader.GetFunctionCode();
	LOGINFO("Entering ProcessMessageFromClient function: funcCode:%i \n" , t_reader.GetFunctionCode());
	switch(t_funcCode)
	{
		LOGINFO("Got Message time to process it\n");
	case SERVER_REQUEST_MESSAGESERVER_ID:
		{
			t_reader.DECODE_ServerRequestMessegeServerID();
			BinaryMessage t_msg1(4096);
			ServerWriterAdapter t_writer(t_msg1);

			m_idSeed++;
			//LOGINFO("YAY RECIVED A ID REQUEST\n");
			LOGINFO("Assigning ID:%i to Connection\n", m_idSeed);
			t_writer.ENCODE_ServerSendMessegeServerID( m_idSeed );
			QueueForWrite(t_msg1);
			break;
		}
	case PING_USER_CHALLENGE_RESP:
		{
			UINT16 t_clientID;
			UINT16 t_challenge;

			t_reader.GetInt2u(t_clientID);
			t_reader.GetInt2u(t_challenge);

			for(std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.begin(); ii != ConnectedUserAccounts.end(); ii++)
			{
				if((*ii)->m_clientID == t_clientID && (*ii)->m_pingchallenge != t_challenge)
				{
					SaveAccount((*ii));
					(*ii)->add_to_trash();
					ConnectedUserAccounts.erase(ii);
					break;
				}
			}

			break;
		}
	case SERVER_USER_LOGIN:
		{
			ProcessUserLogin(t_reader , t_msg);
			break;
		}
	case SERVER_USER_LOGOUT:
		{
			UINT16 clientID;
			UINT16 strLen;
			std::string username;

			t_reader.GetInt2u(clientID);
			//t_reader.GetInt2u(strLen);
			//t_reader.GetRawBytes(username , strLen);


			LOGINFO("Request logout by ClientID:%i \n" , clientID);
			//t_account->m_userName = username;
			for(std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.begin(); ii != ConnectedUserAccounts.end(); ii++)
			{
				if((*ii)->m_clientID == clientID)
				{
					SaveAccount((*ii));
					(*ii)->add_to_trash();
					ConnectedUserAccounts.erase(ii);
					break;
				}
			}
			break;
		}
	case SERVER_CLIENT_FORCE_CLOSE:
		{
			UINT16 clientID;
			t_reader.GetInt2u(clientID);
			for(std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.begin(); ii != ConnectedUserAccounts.end(); ii++)
			{
				if((*ii)->m_clientID == clientID)
				{
					SaveAccount((*ii));
					(*ii)->add_to_trash();
					ConnectedUserAccounts.erase(ii);
					break;
				}
			}

			break;
		}
	default:
		{
			if(t_funcCode <= EVENT_SPECFIC_MESSAGE_HIGH && t_funcCode >= EVENT_SPECFIC_MESSAGE_LOW)
			{
				m_EventAgent->ProcessEventAgentData(*this , t_msg);
				break;
			}
			else{
				LOGINFO("ERROR! WHILE DECODING\n");
				break;
			}

		}
		LOGINFO("TESTING FOR SEG FAULT: Leaving ProcessMessageFromClient()\n");
	};
}

void UserAgent::ProcessUserLogin(ServerReaderAdapter& t_reader , BinaryMessage& t_msg)
{
	LOGINFO("USER_REQUEST_LOGIN\n");
	UINT16 t_strLen;
	UINT16 t_strLen1;
	UINT16 t_clientID;
	UINT16 t_pingchallenge;
	std::string t_userName;
	std::string t_passwrd;
	t_reader.GetInt2u(t_clientID);
	t_reader.GetInt2u(t_pingchallenge);
	t_reader.GetInt2u(t_strLen);
	t_reader.GetRawBytes(t_userName , t_strLen);
	t_reader.GetInt2u(t_strLen1);
	t_reader.GetRawBytes(t_passwrd , t_strLen1);	

	UserAccount* t_account = new UserAccount;
	t_account->m_userName = t_userName;
	t_account->m_passwrd  = t_passwrd;
	//if(GlobalDataBaseConnection->ValidateUser(t_userName , t_passwrd))
//	{
		//GlobalDataBaseConnection->GetUserData(t_account->m_userName , (*t_account));
	//}
	
	t_account->m_pingchallenge = t_pingchallenge;
	UserAccount* verifyacct = GetAccount(t_userName);

	if(verifyacct != NULL && verifyacct->m_userName == t_account->m_userName && verifyacct->m_passwrd == t_account->m_passwrd)
	{

		std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.find(t_account);
		if(ii == ConnectedUserAccounts.end())
		{

			t_account->m_player = verifyacct->m_player;
			
			UINT16 t_id = verifyacct->GetID();
			t_account->m_clientID = t_id;
			t_account->m_player->SetID(t_id);
			t_account->m_userID = t_id;
			LOGINFO("LOGIN SUCCEFUL\n" ); 
			BinaryMessage t_msg(4096);
			ServerWriterAdapter t_writer(t_msg);
			t_writer.AddInt2u(USER_LOGIN_SUCCESS);
			t_writer.AddInt2u(t_clientID);
			t_writer.AddInt2u(t_account->m_userID);
			t_writer.AddInt2u(t_account->m_player->GetPlayerScore());
			t_writer.AddInt2u(t_account->m_player->GetPlayerBalance());
			std::set<MyEvents*> t_myEvents = t_account->m_player->GetPlayerEvents();
			std::set<Shares*> t_myShares = t_account->m_player->GetShares();
			UINT16 myEventsCounter = t_myEvents.size();
			UINT16 mySharesCounter = t_myShares.size();
			t_writer.AddInt2u(myEventsCounter);
			LOGINFO("Getting Ready To Write MyEvents adding:%i events\n" , myEventsCounter);
			if(myEventsCounter > 0)
			{
				for(std::set<MyEvents*>::iterator iii = t_myEvents.begin(); iii != t_myEvents.end(); ++iii)
				{
					t_writer.AddInt2u((*iii)->m_eventID);
					t_writer.AddInt2u((*iii)->m_grpID);
					t_writer.AddInt2u((*iii)->m_eventBalance);
				}
			}
			LOGINFO("Wrote MyEvents\n");
			t_writer.AddInt2u(mySharesCounter);
			LOGINFO("Getting ready to write %i shares to user account\n" , mySharesCounter);
			if(mySharesCounter > 0)
			{
				for(std::set<Shares*>::iterator i = t_myShares.begin(); i != t_myShares.end(); ++i)
				{
					UINT16 t_compID1 = (*i)->GetCompetitorID();
					UINT16 t_sharecnt1  = (*i)->GetShareCount();
					UINT16 t_eventID1 = (*i)->GetEventID();
					UINT16 t_grpID1 = (*i)->GetGrpID();
					t_writer.AddInt2u(t_compID1);
					t_writer.AddInt2u(t_sharecnt1);
					t_writer.AddInt2u(t_eventID1);
					t_writer.AddInt2u(t_grpID1);
				}
			}
			//Sned the orders pertaining to the player from the group
			Player* t_player = t_account->m_player;
			std::vector<Order*> t_playerOrders = t_player->GetPlayerOrders();
			UINT16 t_orderSize = t_player->GetPlayerOrders().size();
			t_writer.AddInt2u(t_orderSize);
			if(t_orderSize > 0)
			{
				for(std::vector<Order*>::iterator xx = t_playerOrders.begin(); xx != t_playerOrders.end(); ++xx)
				{
					//(*xx)->SetOwnerID(t_clientID);
					t_writer.AddInt2u((*xx)->GetID());
					t_writer.AddInt2u((*xx)->GetEventID());
					t_writer.AddInt2u((*xx)->GetGrpID());
					t_writer.AddInt2u((*xx)->GetOwnerID());
					t_writer.AddInt2u((*xx)->GetCompetitorIDForOrder());
					t_writer.AddInt2u((*xx)->GetTypeOfOrder());
					t_writer.AddInt2u((*xx)->GetQtyOfOrder());
					t_writer.AddInt2u((*xx)->GetPriceOfOrder());
					t_writer.AddInt2u((*xx)->GetStatus());
					t_writer.AddInt4s((*xx)->GetTime());
				}
			}

			LOGINFO("Wrote MyShares\n");
			ConnectedUserAccounts.insert(t_account);
			QueueForWrite(t_msg);
		}
		else{ 
			LOGINFO("User already logged in\n");
			BinaryMessage t_msg(4096);
			ServerWriterAdapter t_writer(t_msg);
			t_writer.AddInt2u(USER_LOGIN_FAIL);
			t_writer.AddInt2u(t_clientID);
			QueueForWrite(t_msg);
		}
	}
	else{
		BinaryMessage t_msg(4096);
		ServerWriterAdapter t_writer(t_msg);
		t_writer.AddInt2u(USER_LOGIN_FAIL);
		t_writer.AddInt2u(t_clientID);
		QueueForWrite(t_msg);
		LOGINFO("Invalid Crudentials: \n");
	}
}
