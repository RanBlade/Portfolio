#include "marketmakerstate.h"

MarketMakerState::MarketMakerState()
{

}

MarketMakerState::~MarketMakerState()
{

}

void MarketMakerState::AddEventToState(Event* t_event)
{

  /*
 eventContainerType::iterator t_iter = m_events.find(t_event);
  
  if(t_iter != m_events.end())
  {
    LOGINFO("ERROR EVENT:%i ID ALREADY EXISISTS\n" , t_event);
    return;
  }
  else{
    LOGINFO("Added Event:%i to State\n" , t_event);
    m_events.insert(t_event);
    return;
  }*/
}

void MarketMakerState::AddGroupToEvent(Group* t_grp, Event* t_event)
{

  /*
  eventContainerType::iterator t_iter = m_events.find(t_event);
  
  if(t_iter != m_events.end())
  {
    LOGINFO("Added Group:%i to Event:%i in State\n" , t_group , t_event);
    m_eventGroups.insert( pairUINT16Type(t_event, t_group));
    return;
  }
  else{
    LOGINFO("ERROR NO EVENT IN STATE EVENTID:%i\n" , t_event);
    return;
  }*/
}
void MarketMakerState::AddPlayerToGroup(Player* t_player, Group* t_group)
{
  /*
  eventGroupsContainerType::iterator t_iter;
  for(t_iter = m_eventGroups.begin(); t_iter != m_eventGroups.end(); t_iter++)
  {
    if(t_iter->second == t_group)
    {
      LOGINFO("Added Player:%i to Group:%i in State\n" , t_player , t_group);
      m_GroupPlayers.insert( pairUINT16Type(t_group , t_player));
      return;
    }
    else{
      LOGINFO("ERROR: Group:%i does not belong to a event\n" , t_group);
      return;
    }
  }*/
  
}

void MarketMakerState::AddOrderToPlayer(Order* t_order, Player* t_player)
{
  /*
  groupPlayersContainerType::iterator t_iter;
  for(t_iter = m_GroupPlayers.begin(); t_iter != m_GroupPlayers.end(); t_iter++)
  {
    if(t_iter->second == t_player)
    {
      LOGINFO("Added Order:%i to Player:%i in State\n", t_order , t_player);
      m_PlayerOrders.insert( pairUINT16Type(t_player , t_order));
      return;
    }
    else{
      LOGINFO("ERROR: Player:%i does not belong to a Group\n", t_player);
      return;
    }
  }*/
    
}

bool MarketMakerState::CheckPlayerInState(UINT16 t_player)
{
  /*
  groupPlayersContainerType::iterator ii;
  for(ii = m_GroupPlayers.begin(); ii != m_GroupPlayers.end(); ii++)
  {
    if(ii->second == t_player)
      return true;
  }
  return false;*/
	return false;
}

bool MarketMakerState::CheckEventInState(UINT16 t_event)
{
  /*
  eventContainerType::iterator ii = m_events.find(t_event);
  if(ii != m_events.end())
    return true;
  else
    return false;*/return false;
}

bool MarketMakerState::CheckGroupInState(UINT16 t_group)
{
  /*
  eventGroupsContainerType::iterator ii;
  for(ii = m_eventGroups.begin(); ii != m_eventGroups.end(); ++ii)
  {
    if((*ii).second == t_group)
      return true;
  }
  return false;*/return false;
}
bool MarketMakerState::CheckOrderInState(UINT16 t_order)
{
  /*
  playerOrderContainerType::iterator ii;
  for(ii = m_PlayerOrders.begin(); ii != m_PlayerOrders.end(); ++ii)
  {
    if((*ii).second == t_order)
      return true;
  }
  return false;*/return false;
}

bool MarketMakerState::CheckGroupBelongsToEvent(UINT16 t_group , UINT16 t_event)
{
  /*
  eventGroupsContainerType::iterator testEventii = m_eventGroups.find((t_event));
  if(testEventii != m_eventGroups.end())
  {
    eventGroupsContainerType::iterator ii;
    std::pair< eventGroupsContainerType::iterator , eventGroupsContainerType::iterator> rangeii = m_eventGroups.equal_range(t_event);
    
    for(ii = rangeii.first; ii != rangeii.second; ++ii)
    {
      if((*ii).second == t_group)
	return true;
    }
    return false;
  }
  else{
    LOGINFO("Cant check if Group belongs in event; becasue Event:%i does not exisist in state\n", t_event);
    return false;
  }*/return false;
}
bool MarketMakerState::CheckPlayerBelongsToGroup(UINT16 t_player , UINT16 t_group)
{
  /*
  groupPlayersContainerType::iterator testGroupii = m_GroupPlayers.find(t_group);
  if(testGroupii != m_GroupPlayers.end())
  {
    groupPlayersContainerType::iterator ii;
    std::pair<groupPlayersContainerType::iterator , groupPlayersContainerType::iterator> rangeii = m_GroupPlayers.equal_range(t_group);
  
    for(ii = rangeii.first; ii != rangeii.second; ++ii)
    {
      if((*ii).second == t_player)
	return true;
    }
    return false;
  }
  else{
    LOGINFO("Cant Check if Player belongs in group; becasue Group:%i does not exisist in state\n", t_group);
    return false;
  }*/
  return false;
}
bool MarketMakerState::CheckOrderBelongsToPlayer(UINT16 t_order , UINT16 t_player)
{
  /*
  playerOrderContainerType::iterator testPlayerii = m_PlayerOrders.find(t_player);
  if(testPlayerii != m_PlayerOrders.end())
  {
    playerOrderContainerType::iterator ii;
    std::pair<playerOrderContainerType::iterator , playerOrderContainerType::iterator> rangeii = m_PlayerOrders.equal_range(t_player);
    
    for(ii = rangeii.first; ii != rangeii.second; ++ii)
    {
      if((*ii).second == t_order)
	return true;
    }
    return false;
  }
  else{
    LOGINFO("Cant Check if Order belongs to player; becasue Player:%i does not exist in state\n" , t_player);
    return false;
  }*/return false;
}

