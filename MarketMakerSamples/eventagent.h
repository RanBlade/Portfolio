#ifndef _EVENT_AGENT_H_
#define _EVENT_AGENT_H_

#include "support/support.h"
#include "process_state.h"
#include "socket/socket_base.h"
#include "network/bufferedwriter.h"
#include "network/bufferedreader.h"
#include "binarymessage/binarymessage.h"
#include "event.h"
#include "group.h"
#include "order.h"
#include "marketmakerstate.h"

#include "competitor.h"
#include "log/log.h"
#include "RBHelpers.h"
#include "ServerReaderAdapter.h"
#include "process_state.h"
#include "trash/trashcan.h"
#include "connection.h"

class UserAgent;
class EventAgent : public trashable
{
public:
  typedef std::set<EventAgent*> AgentContainer;
  
  EventAgent(){
    m_agentIDSeed++;
    m_agentID = m_agentIDSeed;
    m_allAgents.insert(this);
    m_connSet = false;
    m_eventDataChanged = false;
    m_marketDataChanged = false;
  }
  virtual ~EventAgent(){
    m_allAgents.erase(this);
    add_to_trash();
  }
  UINT16 GetID(){return m_agentID;}
  //Process is used for the eventagent to view its Orders to determine the market line and 
  //any other processing the Agent may have to do.
  void Process();
  void SetConnection(Connection* t_connection){ m_connection = t_connection;m_connSet = true;}
  void RemoveConnection(Connection* t_connection){ m_connSet = false;}
  //Instance creation functions for all the agent's enitties
  UINT16 CreateEventMM(std::string t_name, UINT16 t_maxGrouPSize, Competitor* t_comp1, Competitor* t_comp2 , INT32 t_epochTime);
  void CreateCompetitor(std::string t_teamName);
  UINT16 CreatePlayerAndAssignToGroup(UINT16 t_eventID, UINT16 t_id, std::string t_name);
  UINT16 CreateGroup();
  UINT16 CreateOrder(UINT16 t_eventID, UINT16 t_ownerID, UINT16 t_type, UINT16 t_competitor, UINT16 t_size, UINT16 t_price);
  
  //State Addition functions for the eventAgent's state it has
  void AddGroupToEvent( Event* t_eventID, Group* t_groupID);
  void AddCompetitorToEvent( UINT16 t_competitor1 , UINT16 t_competitor2 , UINT16 t_event);
  void AddPlayerToGroup(UINT16 t_groupID , UINT16 t_playerID);
  void AddOrderToPlayer(UINT16 t_playerID, UINT16 t_orderID);
  
  //Access functions for the Agent variables using ID
  Event* GetEvent(UINT16 t_id);
  Competitor* GetCompetitor(UINT16 t_eventid,UINT16 t_grpID , UINT16 t_competitorID);
  Group* GetGroup(UINT16 t_eventID, UINT16 t_id);
  Player* GetPlayer(UINT16 t_eventID, UINT16 t_grpid, UINT16 t_playerID);
  Order* GetOrder(UINT16 t_eventid, UINT16 t_id);
  
  void RemoveEvent(UINT16 t_id);
  void RemoveGroup(UINT16 t_id);
  void RemovePlayer(UINT16 t_id);
  void RemoveOrder(UINT16 t_id);
  
  void DoEventSetup(UINT16 t_evenID , std::set<UINT16> t_groupList , std::set<UINT16> t_competitorList);
  void CheckForSuccefulTransactions();
  void CalculateMarket();
  void UpdateClients();//(UINT16 t_clientID , UserAgent& t_agent);
  void UpdateShareValues();
  void UpdateEvents();
  void UpdateAllLeaderBoards();
 
  void PrintAllEvents();
  
  static void ProcessAllAgents()
  {
    m_UpdateEventState.Process();
    m_UpdateMarketState.Process();
    m_UpdateClientDataState.Process();
    
    for(AgentContainer::iterator ii = m_allAgents.begin(); ii != m_allAgents.end(); ++ii)
     {
      (*ii)->Process();
    }
  }
  void CheckForTransactionInAllEvents();
  void ProcessEventTransactions();
  void ProcessEventAgentData(UserAgent& t_useragent, BinaryMessage& t_msg);
  void Process_EventNewPlayer(ServerReaderAdapter& t_reader, UserAgent& t_agent);
  void Process_EventUserCreateOrder(ServerReaderAdapter& t_reader , UserAgent& t_agent);
  void Process_EventUserModifyOrder(ServerReaderAdapter& t_reader , UserAgent& t_agent);
  void Process_EventUserCancelOrder(ServerReaderAdapter& t_reader , UserAgent& t_agent);
  void Process_EventUserJoinEvent(ServerReaderAdapter& t_reader, UserAgent& t_agent);
  void Process_EventPlayerLoadedApp(ServerReaderAdapter& t_reader , UserAgent& t_agent);
  
  //ADMIN FUNCTIONS..
  void Process_AdminCreateEvent(ServerReaderAdapter& t_reader, UserAgent& t_agent);
  void Process_AdminStartEvent(ServerReaderAdapter& t_reader, UserAgent& t_agent);
  void Process_AdminStopEvent(ServerReaderAdapter& t_reader, UserAgent& t_agent);
  void Process_AdminRemoveEvent(ServerReaderAdapter& t_reader, UserAgent& t_agent);
  void Process_AdminAddCompetitor(ServerReaderAdapter& t_reader, UserAgent& t_agent);
  void Process_AdminRemoveCompetitor(ServerReaderAdapter& t_reader, UserAgent& t_agent);
  void Process_AdminEndEventAndProcessWinners(ServerReaderAdapter& , UserAgent&);
protected:
  UINT16                m_agentID;
  static UINT16         m_agentIDSeed;
  EventSet              m_Agentevents;
  Connection*           m_connection;
  bool                  m_connSet;
  bool                  m_eventDataChanged;
  bool                  m_marketDataChanged;
  
  static std::set<EventAgent*> m_allAgents;
  static const UINT16              m_code;
  static ProcessState m_UpdateEventState;
  static ProcessState m_UpdateClientDataState;
  static ProcessState m_UpdateMarketState;
  std::vector<Transaction*> m_allTransactions;
};
#endif