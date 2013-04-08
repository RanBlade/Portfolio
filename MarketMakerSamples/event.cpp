#include "event.h"
#include "log/log.h"

UINT16 Event::m_eventIDSeed = 0;

/*
* FUNCTION NAME: AddPlayerToGroup
* Paramaters: Player*
* 
* Purpose: To add a player to the group 
*/
UINT16 Event::AddPlayerToGroup(Player* t_player)
{
	if(m_activeGroup->GetPlayerCount() < m_maxGroupSize)
	{
		m_activeGroup->AddGroupPlayer(t_player);
		return m_activeGroup->GetID();
	}
	else
	{
		Group* t_group = new Group;
		t_group->SetEventID(m_eventID);
		m_eventGroups.insert(t_group);
		m_activeGroup = t_group;
		m_activeGroup->AddGroupPlayer(t_player);
		return m_activeGroup->GetID();
	}

}

UINT16 Event::GetPlayersInEventCount() const { 
	UINT16 t_totalPlayerCount = 0;
	if(m_eventGroups.empty())
	{
		return 0;
	}
	else{
		for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ii++)
		{
			t_totalPlayerCount += (*ii)->GetPlayerCount();
		}
		return t_totalPlayerCount;
	}
}
Order* Event::GetOrderFromGroup(UINT16 t_grpID , UINT16 t_orderID)
{
	for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ii++)
	{
		if((*ii)->GetID() == t_grpID)
		{
			return (*ii)->GetOrder(t_orderID);
		}
	}
	LOGINFO("Could not locate group for OrderID\n");
	return NULL;
}
bool Event::RemoveOrderFromGroup(UINT16 t_grpID , UINT16 t_orderID)
{
	for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ii++)
	{
		if((*ii)->GetID() == t_grpID)
		{
			(*ii)->RemoveOrder(t_orderID);
			return true;
		}
	}
	LOGINFO("COULD NOT LOCATE GROUP TO REMOVE ORDER FROM\n");
	return false;
}
OrderSetT Event::GetBuyOrders(UINT16 t_grpID)
{
	for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ii++)
	{
		if((*ii)->GetID() == t_grpID)
		{
			return (*ii)->GetBuyOrders();
		}
	}
	//return NULL;
}
OrderSetT Event::GetSellOrders(UINT16 t_grpID)
{
	for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ii++)
	{
		if((*ii)->GetID() == t_grpID)
		{
			return (*ii)->GetSellOrders();
		}
	}
	//return NULL;
}
Group* Event::CheckForGroupInEvent( UINT16 t_id)
{
	Group* t_grp = new Group;
	t_grp->SetGroupIDForCheck(t_id);
	GroupSet::iterator ii = m_eventGroups.find(t_grp);

	if(ii != m_eventGroups.end())
	{
		t_grp->add_to_trash();
		return (*ii);
	}
	else{
		LOGINFO("Group:%i does not exisist in Event:%i\n" , t_grp->GetID(), m_eventID);
		t_grp->add_to_trash();
		return NULL;
	}
}
Player* Event::GetPlayerInEvent(UINT16 t_playerID)
{
	return NULL;
}
void Event::AddSellOrderToGroup(UINT16 t_grpID , Order* t_order)
{
	for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ii++)
	{
		if((*ii)->GetID() == t_grpID)
		{
			(*ii)->AddSellOrder(t_order);
			return;
		}
	}
}
void Event::AddBuyOrderToGroup(UINT16 t_grpID , Order* t_order)
{
	for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ii++)
	{
		if((*ii)->GetID() == t_grpID)
		{
			(*ii)->AddBuyOrder(t_order);
			return;
		}
	}
}
Competitor* Event::CheckForCompetitorInEvent(UINT16 t_grpID , UINT16 t_compID)
{
	Group* t_grp = CheckForGroupInEvent(t_grpID);
	if(t_grp != NULL)
	{
		return t_grp->GetCompetitor(t_compID);
	}
	else{
		LOGINFO("Group:%i does not exisist in Event:%i could not get Competitor:%i \n" , t_grpID , m_eventID , t_compID);
		return NULL;
	}

}

Player* Event::CheckForPlayerInGroupEvent(UINT16 t_grpID , UINT16 t_playerID)
{
	Group* t_grp = CheckForGroupInEvent(t_grpID);

	if(t_grp != NULL)
	{
		return t_grp->GetPlayer(t_playerID);
	}
	else
	{
		LOGINFO("Group:%i does not exisit in this event\n" , t_grpID);
		return NULL;
	}
}

UINT16 Event::GetGroupIDOfPlayerGroup(UINT16 t_id)
{
	Player* t_player = m_activeGroup->GetPlayer(t_id);
	if(t_player != NULL)
	{
		return m_activeGroup->GetID();
	}
	else{
		for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ii++)
		{
			t_player = (*ii)->GetPlayer(t_id);
			if(t_player != NULL)
			{
				return (*ii)->GetID();
			}
		}
		return -1; //if we got here then there is no Group containing this player
	}
}

void Event::AddCompetitorToEvent( Competitor* t_competitor)
{ 
	m_eventCompetitors.insert(t_competitor);
	for(GroupSet::iterator ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ii++)
	{
		Competitor* t_comp1 = new Competitor;
		(*t_comp1) = (*t_competitor);
		(*ii)->AddGroupCompetitor(t_comp1);
	}

}

void Event::ProcessEventEnded(UINT16 t_winnerID)
{
	for(GroupSet::iterator gg = m_eventGroups.begin(); gg != m_eventGroups.end(); ++gg)
	{
		(*gg)->EndEventForGroup(t_winnerID);
	}
}
