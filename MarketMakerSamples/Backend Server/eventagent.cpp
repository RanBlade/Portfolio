#include "eventagent.h"
#include <ServerReaderAdapter.h>
#include "functioncodes.h"
#include <ServerWriterAdapter.h>
#include "time.h"
#include "user.h"
#include "SportLeagues.h"
#include "dbmanager.h"

std::set<EventAgent*> EventAgent::m_allAgents;
ProcessState EventAgent::m_UpdateEventState(1);
ProcessState EventAgent::m_UpdateClientDataState(2);
ProcessState EventAgent::m_UpdateMarketState(1);
UINT16 EventAgent::m_agentIDSeed = 0;
const UINT16 EventAgent::m_code = 17516;


void EventAgent::Process()
{
	if(m_connSet == false)
	{
		m_connection = NULL;
	}
	if(m_UpdateClientDataState._flush_time && m_connSet)
	{
		UpdateClients();
	}
	if(m_UpdateEventState._flush_time)
	{

		LOGINFO("Processing Agent:%i On All Events\n" , m_agentID);
		if(m_connSet && m_eventDataChanged)
		{
			UpdateEvents();
		}
	}
	if(m_UpdateMarketState._flush_time)
	{
		LOGINFO("Processing Agent:%i Market Values\n" , m_agentID);
		CalculateMarket();
		CheckForTransactionInAllEvents();
		ProcessEventTransactions();
		UpdateShareValues();
		UpdateAllLeaderBoards();
	}
}
void EventAgent::UpdateAllLeaderBoards()
{
	for(EventSet::iterator ee = m_Agentevents.begin(); ee != m_Agentevents.end(); ++ee)
	{
		GroupSet t_groups = (*ee)->GetGroups();
		for(GroupSet::iterator gg = t_groups.begin(); gg != t_groups.end(); ++gg)
		{
			(*gg)->UpdateLeaderBoard();
		}
	}
}
void EventAgent::CalculateMarket()
{

	for(EventSet::iterator ii = m_Agentevents.begin(); ii != m_Agentevents.end(); ++ii)
	{

		GroupSet t_groups = (*ii)->GetGroups();
		for(GroupSet::iterator i = t_groups.begin(); i != t_groups.end(); ++i)
		{

			(*i)->CalculateMoneyLine();
		}
	}

	m_marketDataChanged = false;
	m_eventDataChanged = true;
}
void EventAgent::CheckForTransactionInAllEvents()
{
	for(EventSet::iterator ii = m_Agentevents.begin(); ii != m_Agentevents.end(); ++ii)
	{
		GroupSet t_groups = (*ii)->GetGroups();
		for(GroupSet::iterator i = t_groups.begin(); i != t_groups.end(); ++i)
		{
			(*i)->CheckForTransaction();
			//LOGINFO("Checked for transactions in GroupID:%i\n", (*i)->GetID() );
		}
	}
}
void EventAgent::ProcessEventTransactions()
{
	for(EventSet::iterator ii = m_Agentevents.begin(); ii != m_Agentevents.end(); ++ii)
	{
		GroupSet t_groups = (*ii)->GetGroups();
		for(GroupSet::iterator i = t_groups.begin(); i != t_groups.end(); ++i)
		{
			std::vector<Transaction*> TransVec = (*i)->GetAllTransactions();
			for(std::vector<Transaction*>::iterator ii = TransVec.begin(); ii != TransVec.end(); ++ii)
			{
				if(!(*ii)->m_processed)
				{
					UINT16 clientIDBuyer = (*ii)->m_buyerID;
					UINT16 clientIDSeller = (*ii)->m_sellerID;
					UINT16 transactionPrice = (*ii)->m_price;
					UINT16 transactionQty = (*ii)->m_qty;
					UINT16 transactionComp = (*ii)->m_competitorID;
					UINT16 transactionEvent = (*ii)->m_eventID;

					UserAccount* t_playerbuy = GetConnectedAccount(clientIDBuyer);
					Competitor* t_comp = (*i)->GetCompetitor((*ii)->m_competitorID);
					if(t_playerbuy == NULL)
					{
						t_playerbuy = GetAccount(clientIDBuyer);
						t_comp->SetPrice((*ii)->m_price);
						t_playerbuy->m_player->AddShares((*ii)->m_qty , (*ii)->m_competitorID);
						t_playerbuy->m_player->RemoveFrombalanceEscrow(transactionEvent , (transactionPrice * transactionQty));
						t_playerbuy->m_player->RemoveBalanceFromPlayerEvent(transactionEvent , (transactionPrice * transactionQty));
						t_playerbuy->m_player->AddTransaction((*ii));
						SaveAccount(t_playerbuy);
					}
					else
					{
						t_comp->SetPrice((*ii)->m_price);
						t_playerbuy->m_player->AddShares((*ii)->m_qty , (*ii)->m_competitorID);
						t_playerbuy->m_player->RemoveFrombalanceEscrow(transactionEvent , (transactionPrice * transactionQty));
						t_playerbuy->m_player->RemoveBalanceFromPlayerEvent(transactionEvent , (transactionPrice * transactionQty));
						t_playerbuy->m_player->AddTransaction((*ii));
					}
					UserAccount* t_playersell = GetConnectedAccount(clientIDSeller);
					if(t_playersell == NULL)
					{
						t_playersell = GetAccount(clientIDSeller);
						t_playersell->m_player->AddBalanceToPlayerEvent( (*ii)->m_eventID , ((*ii)->m_price * (*ii)->m_qty) );
						t_playersell->m_player->RemoveQtyFromShareEscrow(transactionComp , transactionEvent , transactionQty);
						t_playersell->m_player->RemoveShares(transactionQty , transactionComp);
						t_playersell->m_player->AddTransaction((*ii));
						SaveAccount(t_playersell);
					}
					else{
						t_playersell->m_player->AddBalanceToPlayerEvent( (*ii)->m_eventID , ((*ii)->m_price * (*ii)->m_qty) );
						t_playersell->m_player->RemoveQtyFromShareEscrow(transactionComp , transactionEvent , transactionQty);
						t_playersell->m_player->RemoveShares(transactionQty , transactionComp);
						t_playersell->m_player->AddTransaction((*ii));
					}


					(*ii)->m_processed = true;

					//BinaryMessage t_msg(4096);
					//ServerWriterAdapter t_writer(t_msg);

					//	t_writer.AddInt2u(EVENT_UPDATE_ORDER_STATUS);
					//	t_writer.AddInt2u((*ii)->m_buyerID); //clientID
					//	t_writer.AddInt2u((*ii)->m_buyOrderID); //orderID
					//	t_writer.AddInt2u(ORDER_STATUS_WON); //order status
					//	t_writer.AddInt2u((*ii)->m_qty); //qty of shares in transaction -- will only be used for the left over orders... 
					//	m_connection->QueueForWrite(t_msg);

					//	BinaryMessage t_msg2(4096);
					//		ServerWriterAdapter t_writer2(t_msg2);
					//	t_writer2.AddInt2u(EVENT_UPDATE_ORDER_STATUS);
					//	t_writer2.AddInt2u((*ii)->m_sellerID); //clientID
					//		t_writer2.AddInt2u((*ii)->m_sellOrderID); //orderID
					//		t_writer2.AddInt2u(ORDER_STATUS_SOLD); //order status
					//	t_writer2.AddInt2u((*ii)->m_qty); //qty of shares in transaction -- will only be used for the left over orders... 

					//	m_connection->QueueForWrite(t_msg2);
				}	
			}

		}
	}
}
void EventAgent::UpdateShareValues()
{
	for(EventSet::iterator ii = m_Agentevents.begin(); ii != m_Agentevents.end(); ++ii)
	{
		GroupSet t_groups = (*ii)->GetGroups();
		UINT16 t_eventID = (*ii)->GetID();
		for(GroupSet::iterator gg = t_groups.begin(); gg != t_groups.end(); ++gg)
		{
			PlayerSet t_players = (*gg)->GetAllGroupPlayers();
			for(PlayerSet::iterator pp = t_players.begin(); pp != t_players.end(); ++pp)
			{
				std::set<Shares*> t_shares = (*pp)->GetShares();
				CompetitorSet t_competitors = (*gg)->GetAllGroupCompetitors();
				for(std::set<Shares*>::iterator ss = t_shares.begin(); ss != t_shares.end(); ++ss)
				{
					UINT16 t_compID = (*ss)->GetCompetitorID();
					Competitor* t_comp1 = new Competitor;
					t_comp1->SetIDForCompare(t_compID);
					CompetitorSet::iterator t_comp2 = t_competitors.find(t_comp1);
					if(t_comp2 != t_competitors.end())
					{
						UINT16 t_price = (*t_comp2)->GetPrice();
						(*ss)->SetSharePrice(t_price);
						delete t_comp1;
					}
					
				}
			}
		}
	}

}
UINT16 EventAgent::CreateEventMM(std::string t_name , UINT16 t_maxGrouPSize, Competitor* t_comp1 , Competitor* t_comp2 , INT32 t_epochTime)
{
	Event* t_event = new Event(t_name , t_maxGrouPSize , t_comp1 , t_comp2 ,  t_epochTime);
	//t_event->Init();
	m_Agentevents.insert(t_event);

	return t_event->GetID();
}

UINT16 EventAgent::CreatePlayerAndAssignToGroup(UINT16 t_eventID ,UINT16 t_id, std::string t_name)
{
	Player* t_player = new Player(t_id);
	Event* t_event = new Event;
	t_event->Init();
	t_event->SetIDForSearch(t_eventID);

	EventSet::iterator ii = m_Agentevents.find(t_event);

	if(ii != m_Agentevents.end())
	{
		t_event->add_to_trash();
		(*ii)->AddPlayerToGroup(t_player);
		return (*ii)->GetID();
	}
	else{
		LOGINFO("EventAgent does not control Event:%i\n" , t_eventID);
		t_event->add_to_trash();
		t_player->add_to_trash();
		return -1;
	}


	return t_player->GetID();
}
UINT16 EventAgent::CreateGroup()
{
	/*Group* t_group = new Group;
	m_allAgentGroups.insert(t_group);

	return t_group->GetID();*/
	return 0;
}
UINT16 EventAgent::CreateOrder(UINT16 t_eventID , UINT16 t_ownerID , UINT16 t_type ,UINT16 t_competitor, UINT16 t_size , UINT16 t_price)
{
	Order* t_order = new Order(t_ownerID , t_type , t_competitor , t_size , t_price);

	Event* t_event = new Event;
	t_event->SetIDForSearch(t_eventID);
	EventSet::iterator ii = m_Agentevents.find(t_event);
	if(ii != m_Agentevents.end())
	{
		t_event->add_to_trash();
		// (*ii)->AddOrderToEvent(t_order);
		return (*ii)->GetID();
	}
	else{
		LOGINFO("EventAgent does not control Event:%i\n" , t_eventID);
		t_event->add_to_trash();
		t_order->add_to_trash();
		return -1;
	}
	/*
	Order* t_order = new Order(t_type , t_competitor , t_size, t_price);
	m_allAgentOrders.insert(t_order);

	return t_order->GetID();*/

}

Event* EventAgent::GetEvent(UINT16 t_id)
{
	Event* t_event = new Event;
	t_event->SetIDForSearch(t_id);

	EventSet::iterator ii = m_Agentevents.find(t_event);

	if(ii != m_Agentevents.end())
	{
		t_event->add_to_trash();
		return (*ii);
	}
	else{
		LOGINFO("EventAgent does not control Event:%i\n" , t_id);
		t_event->add_to_trash();
		return NULL;
	}
	/*for(EventSet::iterator ii = m_AgentState.m_allEvents.begin(); ii != m_AgentState.m_allEvents.end(); ii++)
	{
	if((*ii)->GetID() == t_id)
	{
	return (*ii);
	}
	}
	return 0;*/
}
Competitor* EventAgent::GetCompetitor(UINT16 t_eventid, UINT16 t_grpID , UINT16 t_competitorID)
{
	Event* t_event = GetEvent(t_eventid);

	return t_event->CheckForCompetitorInEvent(t_grpID , t_competitorID);
	/*for(CompetitorSet::iterator ii = m_allCompetitors.begin(); ii != m_allCompetitors.end(); ii++)
	{
	if((*ii)->GetID() == t_eventid)
	{
	return (*ii);
	}
	}
	return 0;*/
}

Group* EventAgent::GetGroup(UINT16 t_eventID , UINT16 t_id)
{
	Event* t_event = GetEvent(t_eventID);

	return t_event->CheckForGroupInEvent(t_id);
	/*for(GroupSet::iterator ii = m_allAgentGroups.begin(); ii != m_allAgentGroups.end(); ii++)
	{
	if((*ii)->GetID() == t_id)
	{
	return (*ii);
	}
	}
	return 0;*/
}

Player* EventAgent::GetPlayer(UINT16 t_eventID , UINT16 t_grpid , UINT16 t_playerID)
{
	Event* t_event = GetEvent(t_eventID);

	return t_event->CheckForPlayerInGroupEvent(t_grpid , t_playerID);
	/*for(PlayerSet::iterator ii = m_allAgentPlayers.begin(); ii != m_allAgentPlayers.end(); ii++)
	{
	if((*ii)->GetID() == t_id)
	{
	return (*ii);
	}
	}
	return 0;*/
}
Order* EventAgent::GetOrder(UINT16 t_eventid , UINT16 t_id)
{
	Event* t_event = GetEvent(t_eventid);

	//return t_event->CheckForOrderInEvent(t_id);
	/*for(OrderSet::iterator ii = m_allAgentOrders.begin(); ii != m_allAgentOrders.end(); ii++)
	{
	if((*ii)->GetID() == t_id)
	{
	return (*ii);
	}
	}
	return 0;*/
	return 0;
}

void EventAgent::RemoveEvent(UINT16 t_id)
{
	Event* t_event = GetEvent(t_id);

	EventSet::iterator ii = m_Agentevents.find(t_event);

	if(ii != m_Agentevents.end())
	{
		(*ii)->add_to_trash();
		m_Agentevents.erase(ii);
	}
	else{
		LOGINFO("EventAgent does not control Event:%i\n" , t_id);
	}
	/*
	for(std::set<Event*>::iterator ii = m_allAgentEvents.begin(); ii != m_allAgentEvents.end(); ++ii)
	{
	if((*ii)->GetID() == t_id)
	{
	Event* p = *ii;
	m_allAgentEvents.erase( ii );
	delete( p );
	return;
	}
	} */

}
void EventAgent::RemoveGroup(UINT16 t_id)
{

	/*for(std::set<Group*>::iterator ii = m_allAgentGroups.begin(); ii != m_allAgentGroups.end(); ii++)
	{
	if((*ii)->GetID() == t_id)
	{
	Group* p = *ii;
	m_allAgentGroups.erase(ii);
	delete( p );
	return;
	}
	}*/
}
void EventAgent::RemovePlayer(UINT16 t_id)
{
	/*
	for(std::set<Player*>::iterator ii = m_allAgentPlayers.begin(); ii != m_allAgentPlayers.end(); ii++)
	{
	if((*ii)->GetID() == t_id)
	{
	Player* p = *ii;
	m_allAgentPlayers.erase(ii);
	delete( p );
	return;
	}
	}*/
}
void EventAgent::RemoveOrder(UINT16 t_id)
{

	/*for(std::set<Order*>::iterator ii = m_allAgentOrders.begin(); ii != m_allAgentOrders.end(); ii++)
	{
	if((*ii)->GetID() == t_id)
	{
	Order* p = *ii;
	m_allAgentOrders.erase(ii);
	delete( p );
	return;
	}
	}*/
}
void EventAgent::PrintAllEvents()
{
	for(EventSet::iterator ii = m_Agentevents.begin(); ii != m_Agentevents.end(); ii++)
	{
		std::string t_status;
		if((*ii)->GetStatus() == -1)
			t_status = "Finished";
		else if((*ii)->GetStatus() == -2)
			t_status = "Not Set";
		else if((*ii)->GetStatus() == 0) 
			t_status = "Running";
		else if((*ii)->GetStatus() > 0)
			t_status = "Not Started";
		std::cout << "Event: name:" << (*ii)->GetName() << " | ";
		LOGINFO("ID: %i | Groups: %i | " , (*ii)->GetID() , (*ii)->GetGroupCount());
		std::cout << "Status: " << t_status << " | ";
		(*ii)->PrintCompetitorInfo();


	}
}
void EventAgent::ProcessEventAgentData(UserAgent &t_useragent, BinaryMessage &t_msg)
{
	ServerReaderAdapter t_reader(t_msg);
	UINT16 t_funcCode = t_reader.GetFunctionCode();
	LOGINFO("EventAgent Processing Data from Player\n");

	switch(t_funcCode)
	{
	case EVENT_START:
		{
			break;
		}
	case EVENT_END:
		{
			break;
		}
	case EVENT_TRANSACTION:
		{
			break;
		}
	case EVENT_USER_CREATE_ORDER:
		{
			Process_EventUserCreateOrder(t_reader, t_useragent);
			break;
		}
	case EVENT_USER_CANCEL_ORDER:
		{
			Process_EventUserCancelOrder(t_reader , t_useragent);
			break;
		}
	case EVENT_USER_MODIFY_ORDER:
		{
			Process_EventUserModifyOrder(t_reader , t_useragent);
			break;
		}
	case EVENT_PLAYER_JOINS_GROUP:
		{
			break;
		}
	case EVENT_PLAYER_LEAVES_GROUP:
		{
			break;
		}
	case EVENT_PLAYER_JOINS_EVENT:
		{
			Process_EventUserJoinEvent(t_reader , t_useragent);
			break;
		}
	case EVENT_PLAYER_QUITS_EVENT:
		{
		}
	case EVENT_EVENT_CREATES_GROUP:
		{
		}
	case EVENT_EVENT_REMOVES_GROUP:
		{
		}
	case EVENT_EVENT_UPDATES_MARKET:
		{
		}
	case EVENT_NEW_PLAYER:
		{
			// Process_EventUserJoinEvent(t_useragent , t_msg);
			break;
		}
	case EVENT_PLAYER_LOGIN:
		{
		}
	case EVENT_PLAYER_LOADED_APP:
		{
			Process_EventPlayerLoadedApp(t_reader , t_useragent);
			break;
		}
	case ADMIN_CREATE_EVENT:
		{
			Process_AdminCreateEvent(t_reader , t_useragent);
			break;
		}
	case ADMIN_REMOVE_EVENT:
		{
			Process_AdminRemoveEvent(t_reader, t_useragent);
			break;
		}
	case ADMIN_START_EVENT:
		{
			Process_AdminStartEvent(t_reader , t_useragent);
			break;
		}
	case ADMIN_STOP_EVENT:
		{
			Process_AdminStopEvent(t_reader , t_useragent);
			break;
		}
	case ADMIN_ADD_COMPETITOR:
		{
		}
	case ADMIN_REMOVE_COMPETITOR:
		{
		}

	}
	LOGINFO("Leaving EventAgent Message Processor\n");
}

void EventAgent::Process_EventNewPlayer(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	UINT16 t_agentID;
	UINT16 t_eventID;
	UINT16 t_groupID;
	UINT16 t_playerID;
	std::string t_name;

	t_reader.GetInt2u(t_agentID);
	t_reader.GetInt2u(t_eventID);
	t_reader.GetInt2u(t_groupID);
	t_reader.GetInt2u(t_playerID);
	t_reader.GetStr(t_name);


	if(t_playerID != 0)
	{
		LOGINFO("ERROR PLAYER ALREADY HAS ID\n");
	}
	else{
		Event* t_event = GetEvent(t_eventID);
		t_playerID = CreatePlayerAndAssignToGroup(t_eventID, t_playerID , t_name);
		t_groupID = t_event->GetGroupIDOfPlayerGroup(t_playerID);

		BinaryMessage t_msg1(4096);
		ServerWriterAdapter t_writer(t_msg1);
		t_writer.AddInt2u(EVENT_NEW_PLAYER);
		t_writer.AddInt2u(t_agentID);
		t_writer.AddInt2u(t_eventID);
		t_writer.AddInt2u(t_groupID);
		t_writer.AddInt2u(t_playerID);
	}  
}
void EventAgent::Process_EventUserJoinEvent(ServerReaderAdapter& t_reader, UserAgent& t_agent)
{
	UINT16 t_agentID;
	UINT16 t_eventID;
	UINT16 t_groupID;
	UINT16 t_clientID;
	LOGINFO("Processing Event User Join Event\n");

	t_reader.GetInt2u(t_agentID);
	t_reader.GetInt2u(t_eventID);
	t_reader.GetInt2u(t_groupID);
	t_reader.GetInt2u(t_clientID);

	UserAccount* t_account = GetConnectedAccount(t_clientID);
	if(t_account != NULL && t_agentID == m_agentID)
	{
		LOGINFO("Account is valid\n");
		Event* t_event = GetEvent(t_eventID);
		// Player* t_player = &(t_account->m_player);
		CompetitorSet t_competitors = t_event->GetActiveGroup()->GetAllGroupCompetitors();
		UINT16 t_startingShareBalance = 0;
		UINT16 t_grpID = t_event->AddPlayerToGroup(t_account->m_player);
		MyEvents* t_MyEvents = new MyEvents;
		t_MyEvents->m_eventBalance = STARTING_BALANCE;// + t_startingShareBalance;
		t_MyEvents->m_eventID = t_event->GetID();
		t_MyEvents->m_grpID = t_grpID;
		t_account->m_player->AddEventToPlayerList(t_MyEvents);

		for(CompetitorSet::iterator ii = t_competitors.begin(); ii != t_competitors.end(); ii++)
		{
			Shares* t_share = new Shares((*ii)->GetID() , MAX_STARTING_SHARES , (*ii)->GetPrice());
			t_share->SetEventID(t_event->GetID());
			t_share->SetGrpID(t_grpID);
			t_account->m_player->AddShares(t_share);
			
			t_startingShareBalance += t_share->GetValueOfAllShares();
		}
		UINT16 t_totalBalance = (t_MyEvents->m_eventBalance * t_startingShareBalance);
		//ClientID
		//eventID
		//share1id
		//shareCOUNT
		//share1qty
		//share2id
		//share2qty
		BinaryMessage t_msg(4096);
		ServerWriterAdapter t_writer(t_msg);

		t_writer.AddInt2u(EVENT_PLAYER_JOINS_EVENT);
		t_writer.AddInt2u(t_account->m_player->GetID());
		t_writer.AddInt2u(t_MyEvents->m_eventID);
		t_writer.AddInt2u(t_MyEvents->m_grpID);
		t_writer.AddInt2u(t_totalBalance);

		std::set<Shares*> t_shares = t_account->m_player->GetShares();
		UINT16 t_shareCount = t_shares.size();
		t_writer.AddInt2u(t_shareCount);
		LOGINFO("ADDING %i SHARES <------------------------------------------------------\n" , t_shareCount);
		for(std::set<Shares*>::iterator iii = t_shares.begin(); iii != t_shares.end(); iii++)
		{
			LOGINFO("Adding shares with compID:%i and qty:%i \n" , (*iii)->GetCompetitorID() , (*iii)->GetShareCount());
			t_writer.AddInt2u((*iii)->GetCompetitorID());
			t_writer.AddInt2u((*iii)->GetShareCount());
		}
		m_eventDataChanged = true;
		t_agent.QueueForWrite(t_msg);

	}
	else
	{
	}

}
void EventAgent::Process_EventUserCreateOrder(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	LOGINFO("Creating ORder for User\n");
	UINT16 t_agentID;
	UINT16 t_eventID;
	UINT16 t_groupID;
	UINT16 t_clientID;
	UINT16 t_orderType;
	UINT16 t_competitorID;
	UINT16 t_shareQty;
	UINT16 t_price;

	t_reader.GetInt2u(t_agentID);
	t_reader.GetInt2u(t_eventID);
	t_reader.GetInt2u(t_groupID);
	t_reader.GetInt2u(t_clientID);
	t_reader.GetInt2u(t_orderType);
	t_reader.GetInt2u(t_competitorID);
	t_reader.GetInt2u(t_shareQty);
	t_reader.GetInt2u(t_price);
	UserAccount* t_account = GetConnectedAccount(t_clientID);

	if(t_account != NULL && t_agentID == m_agentID)
	{
		Shares* t_shareOfAccount = t_account->m_player->CheckForSharesOfCompetitor(t_competitorID);
		if(t_shareOfAccount != NULL)
		{
			Event* t_event = GetEvent(t_eventID);
			Group* t_group = t_event->CheckForGroupInEvent(t_groupID);
			Competitor* t_comp = t_group->GetCompetitor(t_competitorID);

			//UINT16 t_ownerID , UINT16 t_type, UINT16 t_competitor, UINT16 t_size, UINT16 t_price
			Order* t_order = new Order(t_clientID , t_orderType , t_competitorID , t_shareQty , t_price);
			t_order->Init();
			t_order->SetEventID(t_eventID);
			t_order->SetGrpID(t_groupID);
			t_order->SetStatus(ORDER_STATUS_OPEN);
			LOGINFO("Created new order with ID:%i | ClientID:%i | OrderType:%i | competitorID:%i | qty:%i | price:%i\n" , t_order->GetID() , t_order->GetOwnerID() , t_order->GetTypeOfOrder() , t_order->GetCompetitorIDForOrder() , t_order->GetAmountofContractsInOrder() , t_order->GetPriceOfOrder()); 
			if(t_orderType == ORDER_TYPE_BUY)
			{
				UINT16 t_testBalance = (t_order->GetQtyOfOrder() * t_price) + t_account->m_player->GetPlayerBalanceEscrow(t_eventID);
				if(t_account->m_player->GetPlayerBalanceEscrow(t_eventID) < t_account->m_player->GetPlayerEventBalance(t_eventID)
					&& t_testBalance < t_account->m_player->GetPlayerEventBalance(t_eventID) )
				{
					//UINT16 t_maxPrice = t_comp->GetMarketLineHI();
					//if(t_price > t_maxPrice){
					//t_price = t_maxPrice;
					//t_order->SetPrice(t_price);
					//}
					t_event->AddBuyOrderToGroup(t_groupID , t_order);
					t_account->m_player->AddToBalanceEscrow(t_eventID , (t_price * t_order->GetQtyOfOrder()));
					BinaryMessage t_msg1(4096);
					ServerWriterAdapter t_writer(t_msg1);

					t_writer.AddInt2u(EVENT_USER_CREATE_ORDER);
					t_writer.AddInt2u(t_eventID);
					t_writer.AddInt2u(t_clientID);
					t_writer.AddInt2u(t_order->GetID());
					t_writer.AddInt2u(t_order->GetTypeOfOrder());
					t_writer.AddInt2u(t_order->GetCompetitorIDForOrder());
					t_writer.AddInt2u(t_order->GetQtyOfOrder());
					t_writer.AddInt2u(t_order->GetPriceOfOrder());
					t_writer.AddInt2u(t_order->GetStatus());
					t_writer.AddInt4s(t_order->GetTime());
					//Add order to player of account
					t_account->m_player->AddOrderToPlayer(t_order);

					t_agent.QueueForWrite(t_msg1);
					m_marketDataChanged = true;
				}
				else{
					//send error to client here
				}

			}
			else if(t_orderType == ORDER_TYPE_SELL)
			{
				if(t_shareOfAccount->GetShareCount() >= t_shareQty
					&& (t_shareOfAccount->GetQuanityInOrder() + t_shareQty) <= t_shareOfAccount->GetShareCount() )
				{
					t_event->AddSellOrderToGroup(t_groupID , t_order);
					t_shareOfAccount->AddToQuanityInOrder(t_shareQty);

					BinaryMessage t_msg1(4096);
					ServerWriterAdapter t_writer(t_msg1);

					t_writer.AddInt2u(EVENT_USER_CREATE_ORDER);
					t_writer.AddInt2u(t_eventID);
					t_writer.AddInt2u(t_clientID);
					t_writer.AddInt2u(t_order->GetID());
					t_writer.AddInt2u(t_order->GetTypeOfOrder());
					t_writer.AddInt2u(t_order->GetCompetitorIDForOrder());
					t_writer.AddInt2u(t_order->GetQtyOfOrder());
					t_writer.AddInt2u(t_order->GetPriceOfOrder());
					t_writer.AddInt2u(t_order->GetStatus());
					t_writer.AddInt4s(t_order->GetTime());
					//Add order to player of account
					t_account->m_player->AddOrderToPlayer(t_order);

					t_agent.QueueForWrite(t_msg1);
					m_marketDataChanged = true;
				}
				else
				{
					LOGINFO("User doens't have enough shares to seel this many\n");
				}
			}

			// UpdateClient(t_clientID , t_agent);
		}

		else
		{
			LOGINFO("AgentID  does not exist\n");
		}
	}


}
void EventAgent::Process_EventUserModifyOrder(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	UINT16 agentID;
	UINT16 eventID;
	UINT16 grpID;
	UINT16 clientID;
	UINT16 orderID;
	INT16 actionOnOrder;
	UINT16 Qty;

	t_reader.GetInt2u(agentID);
	t_reader.GetInt2u(eventID);
	t_reader.GetInt2u(grpID);
	t_reader.GetInt2u(clientID);
	t_reader.GetInt2u(orderID);
	t_reader.GetInt2s(actionOnOrder);
	t_reader.GetInt2u(Qty);
	LOGINFO("%i | %i | %i | %i | %i | %i | %i\n" , agentID, eventID ,grpID, clientID, orderID , actionOnOrder , Qty);
	UserAccount* t_account = GetConnectedAccount(clientID);
	LOGINFO("AID:%i\n" , agentID);
	if(t_account != NULL)
		LOGINFO("NOT NULL\n");
	if(agentID == m_agentID && t_account != NULL)
	{
		Event* t_event = GetEvent(eventID);
		Order* t_order = t_event->GetOrderFromGroup(grpID , orderID);
		Shares* t_share = t_account->m_player->CheckForSharesOfCompetitor(t_order->GetCompetitorIDForOrder());
		LOGINFO("Got nessacary info from server\n");
		if(t_order != NULL)
		{
			LOGINFO("the order does exisit on the server\n");
			if(t_order->GetOwnerID() == clientID)
			{
				LOGINFO("OWNERID MATCHED CLIENT\n");
				if(t_order->GetTypeOfOrder() == ORDER_TYPE_BUY)
				{
					if(actionOnOrder == MODIFY_ORDER_QTY_UP)
					{
						//UINT16 addToShareCnt = t_order->GetQtyOfOrder();
						//UINT16 t_compIDofOrder= t_order->GetCompetitorIDForOrder();
						//t_account->m_player->AddShares(addToShareCnt , t_compIDofOrder);

						UINT16 t_balance =  ((Qty * t_order->GetPriceOfOrder()) - t_order->GetTotalCostOfOrder());
						t_order->SetSize(Qty);
						if(t_balance < 0) 
						{
							////////////////////////NEED TO SOLVE FOR THIS!
						}
						//t_account->m_player->RemoveShares(Qty, t_compIDofOrder);
						t_account->m_player->AddToBalanceEscrow(eventID , t_balance);
						LOGINFO("ORDER SIZE NOW:%i | SHARE QTY NOW:%i\n" , t_order->GetQtyOfOrder() , t_share->GetShareCount());
						BinaryMessage t_msg1(4096);
						ServerWriterAdapter t_writer(t_msg1);
						LOGINFO("GETTING READY TO SEND\n");
						t_writer.AddInt2u(SERVER_UPDATE_ORDER_QTY);
						t_writer.AddInt2u(eventID);
						t_writer.AddInt2u(t_order->GetID());
						t_writer.AddInt2u(clientID);
						t_writer.AddInt2u(t_order->GetQtyOfOrder());

						t_agent.QueueForWrite(t_msg1);
						LOGINFO("SENDING BUY_ORDER UPDATED QTY: INCREASED\n");
						m_marketDataChanged = true;
					}
					else if(actionOnOrder == MODIFY_ORDER_QTY_DOWN)
					{

						if(Qty > 0)
						{
							UINT16 t_balance =  (t_order->GetTotalCostOfOrder() - (Qty * t_order->GetPriceOfOrder()));
							t_order->SetSize(Qty);
							t_account->m_player->RemoveFrombalanceEscrow(eventID , t_balance);

							LOGINFO("ORDER SIZE NOW:%i | SHARE QTY NOW:%i\n" , t_order->GetQtyOfOrder() , t_share->GetShareCount());
							BinaryMessage t_msg1(4096);
							ServerWriterAdapter t_writer(t_msg1);
							LOGINFO("GETTING READY TO SEND\n");
							t_writer.AddInt2u(SERVER_UPDATE_ORDER_QTY);
							t_writer.AddInt2u(eventID);
							t_writer.AddInt2u(t_order->GetID());
							t_writer.AddInt2u(clientID);
							t_writer.AddInt2u(t_order->GetQtyOfOrder());

							t_agent.QueueForWrite(t_msg1);
							LOGINFO("SENDING UPDATED QTY: REDUCED\n");
							m_marketDataChanged = true;

						}

					}
					else if(actionOnOrder == MODIFY_ORDER_PRICE_UP)
					{

						if(Qty > 100)
						{
							Qty = 100;
						}
						UINT16 t_balanceDiff = ((Qty * t_order->GetQtyOfOrder()) - t_order->GetTotalCostOfOrder()); 
						t_order->SetPrice(Qty);
						t_account->m_player->AddToBalanceEscrow(eventID , t_balanceDiff);
						LOGINFO("PRICE CHANGE:-%i\n" , Qty);
						BinaryMessage t_msg1(4096);
						ServerWriterAdapter t_writer(t_msg1);

						LOGINFO("GETTING READY TO SEND\n");
						t_writer.AddInt2u(SERVER_UPDATE_ORDER_PRICE);
						t_writer.AddInt2u(eventID);
						t_writer.AddInt2u(t_order->GetID());
						t_writer.AddInt2u(clientID);
						t_writer.AddInt2u(t_order->GetPriceOfOrder());

						t_agent.QueueForWrite(t_msg1);
						LOGINFO("SENDING PRICE QTY: INCREASED\n");
						m_marketDataChanged = true;

					}
					else if(actionOnOrder == MODIFY_ORDER_PRICE_DOWN)
					{

						if(Qty < 1)
						{
							Qty = 1;
						}
						UINT16 t_balanceDiff = (t_order->GetTotalCostOfOrder() - (Qty * t_order->GetQtyOfOrder()));
						t_order->SetPrice(Qty);
						t_account->m_player->RemoveFrombalanceEscrow(eventID, t_balanceDiff);
						LOGINFO("PRICE CHANGE:+%i\n" , Qty);
						BinaryMessage t_msg1(4096);
						ServerWriterAdapter t_writer(t_msg1);
						LOGINFO("GETTING READY TO SEND\n");
						t_writer.AddInt2u(SERVER_UPDATE_ORDER_PRICE);
						t_writer.AddInt2u(eventID);
						t_writer.AddInt2u(t_order->GetID());
						t_writer.AddInt2u(clientID);
						t_writer.AddInt2u(t_order->GetPriceOfOrder());

						t_agent.QueueForWrite(t_msg1);
						LOGINFO("SENDING PRICE QTY: REDUCED\n");
						m_marketDataChanged = true;

					}
				}       
				else if(t_order->GetTypeOfOrder() == ORDER_TYPE_SELL)
				{
					if(actionOnOrder == MODIFY_ORDER_QTY_UP)
					{
						LOGINFO("QTY:%i\n" , Qty);
						//UINT16 addToShareCnt = t_order->GetQtyOfOrder();
						UINT16 t_compIDofOrder= t_order->GetCompetitorIDForOrder();
						//t_account->m_player->AddShares(addToShareCnt , t_compIDofOrder);
						//  if(Qty > t_share->GetShareCount() && t_share->GetShareCount() != 0){
						//Qty = t_share->GetShareCount(); // User has 1 share -- send 2 -- shares 1
						// } // SHARE QTY 3 ... ORDER QTY 2 NEW ORDER QTY 3 3 - 2 =1  5 = 3 + 2 ... 
						LOGINFO("Aftermodification QTY:%i\n" , Qty);
						// INT16 t_qty = (Qty - t_order->GetQtyOfOrder()); // 3 - 2 = 1
						// LOGINFO("THIS NUMBER IS ABOUT TO BE ASSIGNED TO SHARE QTD:%i\n" , t_qty);
						// if(t_qty <= 0){
						// t_qty = 0;
						// t_share->SetShareQty(t_qty);
						// }
						if(Qty > t_share->GetQuanityInOrder())
							Qty = t_share->GetQuanityInOrder();
						t_order->SetSize(Qty);
						t_account->m_player->SetShareEscrowQty(t_compIDofOrder , eventID , Qty);
						//t_account->m_player->RemoveShares(Qty , t_compIDofOrder);
						//t_share->SetShareQty(t_qty);
						//t_share->RemoveFromShareQty(t_qty);
						LOGINFO("ORDER SIZE NOW:%i | SHARE QTY NOW:%i\n" , t_order->GetQtyOfOrder() , t_share->GetShareCount());
						BinaryMessage t_msg1(4096);
						ServerWriterAdapter t_writer(t_msg1);
						LOGINFO("GETTING READY TO SEND\n");
						t_writer.AddInt2u(SERVER_UPDATE_ORDER_QTY);
						t_writer.AddInt2u(eventID);
						t_writer.AddInt2u(t_order->GetID());
						t_writer.AddInt2u(clientID);
						t_writer.AddInt2u(t_order->GetQtyOfOrder());

						t_agent.QueueForWrite(t_msg1);
						LOGINFO("SENDING UPDATED QTY: INCREASED\n");
						m_marketDataChanged = true;
					}
					else if(actionOnOrder == MODIFY_ORDER_QTY_DOWN)
					{
						LOGINFO("QTY:%i SELL ORDER QTY DOWN\n" , Qty); 
						if(Qty > 0 ) //&& Qty < t_share->GetShareCount()) // Qty = 3 --- share = 2
						{
							//UINT16 addToShareCnt = t_order->GetQtyOfOrder();
							UINT16 t_compIDofOrder= t_order->GetCompetitorIDForOrder();
							//t_account->m_player->AddShares(addToShareCnt , t_compIDofOrder);
							// TOTAL 5 SHARE QTY 4 + 1 ORDERQTY .. 1 NEW ORDER = 2 - 1 = 1
							//INT16 t_qty = (t_order->GetQtyOfOrder() - Qty); //2 - 1 = 1;
							//LOGINFO("THIS NUMBER IS ABOUT TO BE ASSIGNED TO SHARE QTD:%i\n" , t_qty);
							//if(t_qty < 1)
							// t_qty = 1;
							t_order->SetSize(Qty);
							t_account->m_player->SetShareEscrowQty(t_compIDofOrder , eventID , Qty);
							//t_share->SetShareQty(t_qty);
							LOGINFO("ORDER SIZE NOW:%i | SHARE QTY NOW:%i\n" , t_order->GetQtyOfOrder() , t_share->GetShareCount());
							BinaryMessage t_msg1(4096);
							ServerWriterAdapter t_writer(t_msg1);
							LOGINFO("GETTING READY TO SEND\n");
							t_writer.AddInt2u(SERVER_UPDATE_ORDER_QTY);
							t_writer.AddInt2u(eventID);
							t_writer.AddInt2u(t_order->GetID());
							t_writer.AddInt2u(clientID);
							t_writer.AddInt2u(t_order->GetQtyOfOrder());

							t_agent.QueueForWrite(t_msg1);
							LOGINFO("SENDING UPDATED QTY: REDUCED\n");
							m_marketDataChanged = true;

						}
					}
					else if(actionOnOrder == MODIFY_ORDER_PRICE_UP)
					{

						if(Qty > 100)
						{
							Qty = 100;
						}

						t_order->SetPrice(Qty);
						LOGINFO("PRICE CHANGE:-%i\n" , Qty);
						BinaryMessage t_msg1(4096);
						ServerWriterAdapter t_writer(t_msg1);

						LOGINFO("GETTING READY TO SEND\n");
						t_writer.AddInt2u(SERVER_UPDATE_ORDER_PRICE);
						t_writer.AddInt2u(eventID);
						t_writer.AddInt2u(t_order->GetID());
						t_writer.AddInt2u(clientID);
						t_writer.AddInt2u(t_order->GetPriceOfOrder());

						t_agent.QueueForWrite(t_msg1);
						LOGINFO("SENDING PRICE QTY: INCREASED\n");
						m_marketDataChanged = true;

					}
					else if(actionOnOrder == MODIFY_ORDER_PRICE_DOWN)
					{

						if(Qty < 1)
						{
							Qty = 1;
						}
						t_order->SetPrice(Qty);
						LOGINFO("PRICE CHANGE:+%i\n" , Qty);
						BinaryMessage t_msg1(4096);
						ServerWriterAdapter t_writer(t_msg1);
						LOGINFO("GETTING READY TO SEND\n");
						t_writer.AddInt2u(SERVER_UPDATE_ORDER_PRICE);
						t_writer.AddInt2u(eventID);
						t_writer.AddInt2u(t_order->GetID());
						t_writer.AddInt2u(clientID);
						t_writer.AddInt2u(t_order->GetPriceOfOrder());

						t_agent.QueueForWrite(t_msg1);
						LOGINFO("SENDING PRICE QTY: REDUCED\n");
						m_marketDataChanged = true;
					}
				}
			}
		}

	}
	else
	{
		LOGINFO("ERRO either no account or agent at these ID's\n");
	}

}
void EventAgent::Process_EventUserCancelOrder(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	UINT16 agentID;
	UINT16 eventID;
	UINT16 grpID;
	UINT16 clientID;
	UINT16 orderID;

	t_reader.GetInt2u(agentID);
	t_reader.GetInt2u(eventID);
	t_reader.GetInt2u(grpID);
	t_reader.GetInt2u(clientID);
	t_reader.GetInt2u(orderID);

	if(agentID == m_agentID)
	{
		UserAccount* t_account = GetConnectedAccount(clientID);
		Event* t_event = GetEvent(eventID);
		Order* t_order = t_event->GetOrderFromGroup(grpID , orderID);
		t_order->SetStatus(ORDER_STATUS_CANCELED);
		UINT16 orderType = t_order->GetTypeOfOrder();

		if(orderType == ORDER_TYPE_BUY)
		{
			UINT16 t_price = (t_order->GetPriceOfOrder() * t_order->GetQtyOfOrder());
			t_account->m_player->RemoveFrombalanceEscrow(eventID , t_price);
		}

		if(orderType == ORDER_TYPE_SELL)
		{
			UINT16 t_compID = t_order->GetCompetitorIDForOrder();
			UINT16 t_qty = t_order->GetQtyOfOrder();
			Shares* t_share = t_account->m_player->CheckForSharesOfCompetitor(t_compID);
			t_share->RemoveQuanityInOrder(t_qty);
		}
		bool t_success = t_event->RemoveOrderFromGroup(grpID , orderID);


		if(t_success)
		{
			BinaryMessage t_msg(4096);
			ServerWriterAdapter t_writer(t_msg);

			t_writer.AddInt2u(EVENT_USER_CANCEL_ORDER);
			t_writer.AddInt2u(eventID);
			t_writer.AddInt2u(clientID);
			t_writer.AddInt2u(orderID);




			t_agent.QueueForWrite(t_msg);
			m_marketDataChanged = true;
		}
		else
		{
			LOGINFO("ERROR: NO order at OrderID\n");
		}

	}

}
void EventAgent::Process_EventPlayerLoadedApp(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	UINT16 t_agentID;
	UINT16 t_eventID;
	UINT16 t_groupID;
	UINT16 t_playerID;

	t_reader.GetInt2u(t_agentID);
	t_reader.GetInt2u(t_eventID);
	t_reader.GetInt2u(t_playerID);
	t_reader.GetInt2u(t_groupID);
	LOGINFO("ClientID:%i\n" , t_playerID);
	if(!m_Agentevents.empty())
	{
		//TO-DO: Make it only send 10 events per message. 
		//BinaryMessage t_msg(4096);
		// ServerWriterAdapter t_writer(t_msg);

		UINT16 t_eventCount = m_Agentevents.size();
		t_agentID = GetID();    

		//t_writer.AddInt2u(EVENT_PLAYER_LOADED_APP);
		//t_writer.AddInt2u(t_agentID);
		//t_writer.AddInt2u(1);

		/*
		* AgentID
		* eventCount
		* EventID
		* --competitorCount
		* ---CompetitorID1
		* ---CompetitorID2
		* ---ComeptitorIDn
		* --GroupCount 
		* ---GroupID
		* ---GroupPlayerCount
		* ---GroupCompetitorCount
		* ---GroupCompetitors
		* EventStartTime-EpochBased
		*/

		for(EventSet::iterator ii = m_Agentevents.begin(); ii != m_Agentevents.end(); ii++)
		{
			BinaryMessage t_msg(4096);
			ServerWriterAdapter t_writer(t_msg);
			t_writer.AddInt2u(EVENT_PLAYER_LOADED_APP);
			t_writer.AddInt2u(t_playerID);
			t_writer.AddInt2u(t_agentID);
			t_writer.AddInt2u(1);
			t_writer.AddInt2u((*ii)->GetID());//EVENT ID
			t_writer.AddInt2u((*ii)->GetCompetitorCount()); //how many competitors in the event
			CompetitorSet t_compSet = (*ii)->GetCompetitors(); //List of Competitors in Event

			//LOGINFO("WRITING EVENT\n");
			//LOGINFO("COMPETITOR COUNT:%i FOR EVENT\n" , (*ii)->GetCompetitorCount());
			for(CompetitorSet::iterator iii = t_compSet.begin(); iii != t_compSet.end(); iii++)
			{
				t_writer.AddInt2u((*iii)->GetID());
				t_writer.AddInt2u((*iii)->GetMarketLineHI());
				t_writer.AddInt2u((*iii)->GetsharesatHI());
				t_writer.AddInt2u((*iii)->GetMarketLineLO());
				t_writer.AddInt2u((*iii)->GetSharesAtLO());
				//LOGINFO("WRITING COMPETITOR\n");
			}

			t_writer.AddInt2u((*ii)->GetGroupCount());
			GroupSet t_groupSet = (*ii)->GetGroups(); //List of Groups in EVent  
			for(GroupSet::iterator iiii = t_groupSet.begin(); iiii != t_groupSet.end(); iiii++)
			{
				t_writer.AddInt2u((*iiii)->GetID());
				t_writer.AddInt2u((*iiii)->GetPlayerCount());
				LOGINFO("PlayerCount:%i\n" , (*iiii)->GetPlayerCount());
				CompetitorSet t_competitors = (*iiii)->GetAllGroupCompetitors();
				t_writer.AddInt2u(t_competitors.size());
				for(CompetitorSet::iterator x = t_competitors.begin(); x != t_competitors.end(); x++)
				{
					t_writer.AddInt2u((*x)->GetID());
					t_writer.AddInt2u((*x)->GetMarketLineHI());
					t_writer.AddInt2u((*x)->GetsharesatHI());
					t_writer.AddInt2u((*x)->GetMarketLineLO());
					t_writer.AddInt2u((*x)->GetSharesAtLO());
					//LOGINFO("WRITING COMPETITOR\n");
				}
				t_writer.AddInt2u((*ii)->GetPlayersInEventCount());
				t_writer.AddInt4s((*ii)->GetStatus());
				LOGINFO("Queueing Message_EVENT_PLAYER_LOADED_APP\n"); 
				t_agent.QueueForWrite(t_msg);
			}
			// LOGINFO("Queueing Message_EVENT_PLAYER_LOADED_APP\n"); 
			// t_agent.QueueForWrite(t_msg);
		}
	}
}
////////////////////////////////////////////////////////////////////////////////////////////////////////
//Admin Control Message Functions
void EventAgent::Process_AdminCreateEvent(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	UINT16 t_code;
	UINT16 t_maxSize;
	UINT16 t_strLen;
	std::string t_eventname;
	UINT16 t_competitor1;
	UINT16 t_competitor2;
	INT32 t_startTime;


	t_reader.GetInt2u(t_code);
	t_reader.GetInt2u(t_maxSize);
	t_reader.GetInt2u(t_competitor1);
	t_reader.GetInt2u(t_competitor2);
	t_reader.GetInt4s(t_startTime);
	t_reader.GetInt2u(t_strLen);
	t_reader.GetRawBytes(t_eventname , t_strLen);




	if(t_code == m_code)
	{
		LOGINFO("ADMIN DETECTED: Trying to create Event with name: ");
		std::cout << t_eventname << std::endl;

		Competitor* t_comp1 = new Competitor;
		*t_comp1 = *(GetSportsTeam(t_competitor1));
		Competitor* t_comp2 = new Competitor;
		*t_comp2 = *(GetSportsTeam(t_competitor2));

		if(t_comp1 != NULL && t_comp2 != NULL){
			UINT16 t_eventID = CreateEventMM(t_eventname , t_maxSize , t_comp1 , t_comp2, t_startTime);
			Event t_event = *(GetEvent(t_eventID));
			BinaryMessage t_msg(4096);
			ServerWriterAdapter t_writer(t_msg);
			t_writer.AddInt2u(EVENT_CREATED_BY_ADMIN);
			t_writer.AddInt2u(t_eventID);

			PrintAllEvents();
		//	GlobalDataBaseConnection->AddEventToDatabase(t_event);
			m_eventDataChanged = true;
		}
		else{
			LOGINFO("ERROR CREATING EVENT\n");
		}
	}

}
void EventAgent::Process_AdminStartEvent(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	UINT16 t_code;
	UINT16 t_eventID;


	t_reader.GetInt2u(t_code);
	t_reader.GetInt2u(t_eventID);
	if(t_code == m_code){
		Event* t_event = 0;
		t_event = GetEvent(t_eventID);
		if(t_event == NULL){
			LOGINFO("ADMIN ERROR: No event at this ID\n");
		}
		else{
			t_event->SetStartTime(0);  
			PrintAllEvents();
			m_eventDataChanged = true;
		}


	}

}

void EventAgent::Process_AdminStopEvent(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	UINT16 t_code = 0;
	UINT16 t_eventID = 0;
	UINT16 t_winnerID = 0;

	t_reader.GetInt2u(t_code);
	t_reader.GetInt2u(t_eventID);
	t_reader.GetInt2u(t_winnerID);
	if(t_code == m_code)
	{
		Event* t_event = 0;
		t_event = GetEvent(t_eventID);

		if(t_event == NULL)
		{
			LOGINFO("ADMIND ERROR: No Event at this ID\n");
		}
		else{
			t_event->SetStartTime(-1);
			t_event->ProcessEventEnded(t_winnerID);
			PrintAllEvents();
			m_eventDataChanged = true;
		}
	}
}
void EventAgent::Process_AdminRemoveEvent(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	UINT16 t_code;
	UINT16 t_eventID;

	t_reader.GetInt2u(t_code);
	t_reader.GetInt2u(t_eventID);

	if(t_code == m_code)
	{
		Event* t_event = GetEvent(t_eventID);
		if(t_event != NULL)
		{
			if(t_event->object_active())
			{
				BinaryMessage t_msg(4096);
				ServerWriterAdapter t_writer(t_msg);

				t_writer.AddInt2u(EVENT_REMOVE_EVENT);
				t_writer.AddInt2u(t_event->GetID());

				t_agent.QueueForWrite(t_msg);
				for(std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.begin(); ii != ConnectedUserAccounts.end(); ii++)
				{
					std::set<MyEvents*> t_events = (*ii)->m_player->GetPlayerEvents();
					for(std::set<MyEvents*>::iterator iii = t_events.begin(); iii != t_events.end(); iii++)
					{
						if((*iii)->m_eventID == t_event->GetID())
						{
							LOGINFO("REMOVING EVENT FROM MY EVENTS OF PLAYER\n");
							(*iii)->add_to_trash();
							t_events.erase((*iii));

							SaveAccount((*ii));
							break;
						}
					}
					std::set<Shares*> t_shares = (*ii)->m_player->GetShares();
					LOGINFO("Attempting to remove %i share from UserAccount:%i\n" , (int)t_shares.size(), (*ii)->GetID());
					for(std::set<Shares*>::iterator iiii = t_shares.begin();iiii != t_shares.end(); iiii++)
					{
						if((*iiii)->GetEventID() == t_event->GetID())
						{
							(*iiii)->add_to_trash();
							t_shares.erase((*iiii));

							SaveAccount((*ii));
							break;
						}
					}
				}
				m_Agentevents.erase(t_event);
				t_event->add_to_trash();

				t_agent.QueueForWrite(t_msg);
				LOGINFO("Removing Event:%i\n" , t_event->GetID());
				PrintAllEvents();
				m_eventDataChanged = true;
			}
			else
			{
				LOGINFO("ADMIN ERROR: Sorry event is still active\n");
			}
		}
		else{
			LOGINFO("ADMIN ERROR: This Agent Does not control the event\n");

		}
	}
	else{
		LOGINFO("ADMING ERROR: Invalid Auth CODE\n");
	}
}
void EventAgent::Process_AdminEndEventAndProcessWinners(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	//Need work here.
}
void EventAgent::Process_AdminAddCompetitor(ServerReaderAdapter& t_reader , UserAgent& t_agent)
{
	// UINT16 t_code;
	// UINT16 t_eventID;
	// UINT16 t_competitorID;


}
void EventAgent::UpdateClients()//(UINT16 t_clientID , UserAgent& t_agent)
{
	for(std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.begin(); ii != ConnectedUserAccounts.end(); ii++)
	{
		//t_account->m_player;// = verifyacct->m_player;
		//t_account->m_player->SetID(t_clientID);
		// LOGINFO("LOGIN SUCCEFUL\n" ); 
		BinaryMessage t_msg(4096);
		ServerWriterAdapter t_writer(t_msg);
		t_writer.AddInt2u(UPDATE_CLIENT_INFO);
		t_writer.AddInt2u((*ii)->m_clientID);
		t_writer.AddInt2u((*ii)->m_player->GetPlayerScore());
		t_writer.AddInt2u((*ii)->m_player->GetPlayerBalance());
		std::set<MyEvents*> t_myEvents = (*ii)->m_player->GetPlayerEvents();
		std::set<Shares*> t_myShares = (*ii)->m_player->GetShares();
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
				UINT16 t_balance = (*ii)->m_player->GetPlayerEventBalance((*iii)->m_eventID);
				t_writer.AddInt2u(t_balance);
				LOGINFO("MyEVENTS: ID:%i | grpID:%i | Balance:%i\n" , (*iii)->m_eventID , (*iii)->m_grpID , (*iii)->m_eventBalance);

			}
		}
		LOGINFO("Wrote MyEvents\n");
		t_writer.AddInt2u(mySharesCounter);
		LOGINFO("SHARCOUNTER:%i <----------------------\n" , mySharesCounter );
		if(mySharesCounter > 0)
		{
			for(std::set<Shares*>::iterator i = t_myShares.begin(); i != t_myShares.end(); ++i)
			{
				UINT16 t_compID1 = (*i)->GetCompetitorID();
				UINT16 t_sharecnt1 = (*i)->GetShareCount();
				UINT16 t_eventID1 = (*i)->GetEventID();
				UINT16 t_grpID1 = (*i)->GetGrpID();
				t_writer.AddInt2u(t_compID1);
				t_writer.AddInt2u(t_sharecnt1);
				t_writer.AddInt2u(t_eventID1);
				t_writer.AddInt2u(t_grpID1);
				LOGINFO("SHARES: CompID:%i | ShareCnt:%i | EID:%i | GRPID%i\n" , t_compID1 , t_sharecnt1 , t_eventID1 , t_grpID1);
			}
		}
		//Sned the orders pertaining to the player from the group
		Player* t_player = (*ii)->m_player;
		std::vector<Order*> t_playerOrders = t_player->GetPlayerOrders();
		UINT16 t_orderSize = t_playerOrders.size();
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
				LOGINFO("ORDERS: ID:%i | EID:%i | GRPID:%i | OWNERID:%i | COMPID:%i | TypeOfOdr:%i | QTY:%i | PRICE:%i | TM:NAi\n",
					(*xx)->GetID(),(*xx)->GetEventID(),(*xx)->GetGrpID(),(*xx)->GetOwnerID(),(*xx)->GetCompetitorIDForOrder(),(*xx)->GetTypeOfOrder(),(*xx)->GetQtyOfOrder(),(*xx)->GetPriceOfOrder());
			}
		}
		//std::vector<Transactions*> t_playertransactions = t_player->GetTransactions();
		//UINT16 t_trasnSize = 
		LOGINFO("Wrote MyShares\n");

		m_connection->QueueForWrite(t_msg);
	}
}

void EventAgent::UpdateEvents()
{
	if(!m_Agentevents.empty())
	{
		//TO-DO: Make it only send 10 events per message. 
		//BinaryMessage t_msg(4096);
		//ServerWriterAdapter t_writer(t_msg);

		UINT16 t_eventCount = m_Agentevents.size();
		UINT16 t_agentID = GetID();    

		// t_writer.AddInt2u(USER_AGENT_EVENT_UPDATE_SEND_TO_ALL);
		// t_writer.AddInt2u(m_agentID);
		// t_writer.AddInt2u(t_eventCount);

		/*
		* AgentID
		* eventCount
		* EventID
		* --GroupCount 
		* ---Group Players
		* --competitorCount
		* ---CompetitorID1
		* ---CompetitorID2
		* ---ComeptitorIDn
		* EventStartTime-EpochBased
		*/
		for(EventSet::iterator ii = m_Agentevents.begin(); ii != m_Agentevents.end(); ii++)
		{
			BinaryMessage t_msg(4096);
			ServerWriterAdapter t_writer(t_msg);
			t_writer.AddInt2u(USER_AGENT_EVENT_UPDATE_SEND_TO_ALL);
			t_writer.AddInt2u(t_agentID);
			t_writer.AddInt2u(1);
			t_writer.AddInt2u((*ii)->GetID());//EVENT ID
			t_writer.AddInt2u((*ii)->GetCompetitorCount()); //how many competitors in the event
			CompetitorSet t_compSet = (*ii)->GetCompetitors(); //List of Competitors in Event
			//LOGINFO("WRITING EVENT\n");
			//LOGINFO("COMPETITOR COUNT:%i FOR EVENT\n" , (*ii)->GetCompetitorCount());
			for(CompetitorSet::iterator iii = t_compSet.begin(); iii != t_compSet.end(); iii++)
			{
				t_writer.AddInt2u((*iii)->GetID());
				t_writer.AddInt2u((*iii)->GetMarketLineHI());
				t_writer.AddInt2u((*iii)->GetsharesatHI());
				t_writer.AddInt2u((*iii)->GetMarketLineLO());
				t_writer.AddInt2u((*iii)->GetSharesAtLO());
				//LOGINFO("WRITING COMPETITOR\n");
			}

			t_writer.AddInt2u((*ii)->GetGroupCount());
			LOGINFO("GroupCount:%i \n" , (*ii)->GetGroupCount());
			GroupSet t_groupSet = (*ii)->GetGroups(); //List of Groups in EVent
			for(GroupSet::iterator iiii = t_groupSet.begin(); iiii != t_groupSet.end(); iiii++)
			{
				t_writer.AddInt2u((*iiii)->GetID());
				LOGINFO("GroupID:%i " , (*iiii)->GetID());
				t_writer.AddInt2u((*iiii)->GetPlayerCount());
				LOGINFO("PlayerCount: %i\n" , (*iiii)->GetPlayerCount());

				CompetitorSet t_competitors = (*iiii)->GetAllGroupCompetitors();
				t_writer.AddInt2u(t_competitors.size());
				for(CompetitorSet::iterator x = t_competitors.begin(); x != t_competitors.end(); x++)
				{
					t_writer.AddInt2u((*x)->GetID());
					t_writer.AddInt2u((*x)->GetMarketLineHI());
					t_writer.AddInt2u((*x)->GetsharesatHI());
					t_writer.AddInt2u((*x)->GetMarketLineLO());
					t_writer.AddInt2u((*x)->GetSharesAtLO());
					// LOGINFO("SENDING COMPETITOR INFO IN GROUP: %i | %i | %i | %i | %i\n" , (*x)->GetID(),(*x)->GetMarketLineHI(), (*x)->GetsharesatHI()
					//  , (*x)->GetMarketLineLO() ,(*x)->GetSharesAtLO());
					//LOGINFO("WRITING COMPETITOR\n");

				}
				std::vector<LeaderBoardEntry*> t_leaderboard = (*iiii)->GetLeaderBoard().m_players;
				UINT16 t_leaderboardsize = t_leaderboard.size();
				t_writer.AddInt2u(t_leaderboardsize);
				for(std::vector<LeaderBoardEntry*>::iterator lb = t_leaderboard.begin(); lb != t_leaderboard.end(); ++lb)
				{
					t_writer.AddInt2u((*lb)->m_playerID);
					t_writer.AddInt2u((*lb)->m_score);
				}
			}
			t_writer.AddInt2u((*ii)->GetPlayersInEventCount());
			t_writer.AddInt4s((*ii)->GetStatus());
			LOGINFO("Queueing Message_EVENT_UPDATE_SEND_TO_ALL\n"); 
			m_connection->QueueForWrite(t_msg);
		}
		//   LOGINFO("Queueing USER_AGENT_EVENT_UPDATE_SEND_TO_ALL\n"); 
		//   m_connection->QueueForWrite(t_msg);
	}   
	m_eventDataChanged = false;
}