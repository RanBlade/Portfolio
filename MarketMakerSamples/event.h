#ifndef _EVENT_H_
#define _EVENT_H_

#include "trash/trashcan.h"
#include "support/support.h"
#include "group.h"
#include "competitor.h"
#include "RBHelpers.h"
#include "log/log.h"


class EventTransaction;

//Event class is a data holding class for the EventManager to hold info 
//about the events that it has going on.
class Event : public trashable
{
public:
	Event(std::string t_name , UINT16 t_maxGrpSize , Competitor* t_competitor1 , Competitor* t_competitor2 , INT32 t_epochTime) : m_EventName(t_name) , m_maxGroupSize(t_maxGrpSize) , m_startTime(t_epochTime)
	{
		//m_startTime = -2;
		Init();
		Group* t_grp = new Group();
		t_grp->Init();
		t_grp->SetEventID(m_eventID);
		m_activeGroup = t_grp;
		m_eventGroups.insert(t_grp);

		AddCompetitorToEvent(t_competitor1);
		AddCompetitorToEvent(t_competitor2);
	}
	Event(){} // only use this for searching for events
	~Event()
	{
		LOGINFO("Adding Event to trash\n");
		//add_to_trash();
	}
	void pre_trash()
	{
		ClearEventsAndCompetitors();
		m_eventCompetitors.clear();
		m_eventGroups.clear(); 
	}
	UINT16 GetID() const { return m_eventID;}
	Group* GetActiveGroup() 
	{
		return m_activeGroup;
	}
	void Init()
	{
		m_eventIDSeed++;
		m_eventID = m_eventIDSeed;
	}
	//Basic functions to add players.
	void AddGroupToEvent( Group* t_grp){ m_eventGroups.insert(t_grp); }
	UINT16 AddPlayerToGroup(Player* t_player);
	void AddCompetitorToEvent( Competitor* t_competitor);
	//void AddOrderToEvent(Order* t_order){ m_eventBuyOrders.insert(t_order);}

	//Verification fucntions to prove a group exist... for simplicity if it does its going to return the instance.
	Group* CheckForGroupInEvent(UINT16 t_id);
	Competitor* CheckForCompetitorInEvent(UINT16 t_grpID , UINT16 t_compID);
	//Order* CheckForOrderInEvent(UINT16 t_id);
	Player* CheckForPlayerInGroupEvent(UINT16 t_grpID , UINT16 t_playerID);
	Player* GetPlayerInEvent(UINT16 t_playerID);

	UINT16 GetGroupIDOfPlayerGroup(UINT16 t_id);
	void SetIDForSearch(UINT16 t_id){ m_eventID = t_id;}

	void SetStartTime(INT32 t_starTime){ m_startTime = t_starTime;}
	INT32 GetStatus() const { return m_startTime;}
	CompetitorSet GetCompetitors() const { return m_eventCompetitors;}
	GroupSet GetGroups() const { return m_eventGroups;}
	Group* GetActiveGroup() const { return m_activeGroup;}
	std::string GetName()const { return m_EventName;}
	UINT16 GetGroupCount()const { if(m_eventGroups.empty()){return 0;} else{ return m_eventGroups.size();}}
	UINT16 GetPlayersInEventCount() const;
	UINT16 GetCompetitorCount() const { return m_eventCompetitors.size();}
	Order* GetOrderFromGroup(UINT16 , UINT16);
	bool   RemoveOrderFromGroup(UINT16 , UINT16);
	OrderSetT GetBuyOrders(UINT16);
	OrderSetT GetSellOrders(UINT16);
	void AddSellOrderToGroup(UINT16 ,Order* t_order);
	void AddBuyOrderToGroup(UINT16 , Order* t_order);
	void ProcessEventEnded(UINT16);
	void EndEvent();

	void PrintCompetitorInfo(){
		for(CompetitorSet::iterator ii = m_eventCompetitors.begin(); ii != m_eventCompetitors.end(); ii++)
		{
			std::cout << " Team: " << (*ii)->GetCompetitorName() << " | ";
		}
		std::cout << std::endl;
	}
	void ClearEventsAndCompetitors()
	{
		for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ++ii)
		{
			(*ii)->add_to_trash();
		}
		for(CompetitorSet::iterator i = m_eventCompetitors.begin(); i != m_eventCompetitors.end(); ++i)
		{
			(*i)->add_to_trash();
		}

	}
private:
	std::string   m_EventName;
	GroupSet      m_eventGroups;
	CompetitorSet m_eventCompetitors; // this is for data storage purposes only do not use data or modify data from this Set.
	UINT16        m_eventID;
	static UINT16 m_eventIDSeed;

	Group*        m_activeGroup;

	UINT16        m_maxGroupSize;
	INT32         m_startTime;


};
#endif