#ifndef MARKETMAKERSTATE_H
#define MARKETMAKERSTATE_H

#include "support/support.h"
#include "log/log.h"
#include "player.h"
#include "group.h"
#include "event.h"
#include "RBHelpers.h"

class MarketMakerState
{

public:

  Event* m_Event;
  
  
  
  
  
  
  //Going to be depreciated after tonight.
  //std::set<UINT16> m_events;
  //std::multimap<UINT16 , UINT16> m_eventGroups;
  //std::multimap<UINT16 , UINT16> m_GroupPlayers;
  //std::multimap<UINT16 , UINT16> m_PlayerOrders;
  
  MarketMakerState();
  virtual ~MarketMakerState();
  
  void AddEventToState(Event* t_event);
  void AddGroupToEvent(Group* t_grp , Event* t_event);
  void AddPlayerToGroup(Player* t_player , Group* t_group);
  void AddOrderToPlayer(Order* t_order , Player* t_player);
  
  Player* GetPlayerFromState(UINT16 t_grpID , UINT16 t_playerID);
  Group*  GetGroupFromState(UINT16 t_id);
  Competitor* GetCompetitorFromState( UINT16 t_id);
  
  
  bool CheckPlayerInState(UINT16 t_player);
  bool CheckEventInState(UINT16 t_event);
  bool CheckGroupInState(UINT16 t_group);
  bool CheckOrderInState(UINT16 t_order);
  
  bool CheckGroupBelongsToEvent(UINT16 t_group , UINT16 t_event);
  bool CheckPlayerBelongsToGroup(UINT16 t_player , UINT16 t_group);
  bool CheckOrderBelongsToPlayer(UINT16 t_order , UINT16 t_player);
  
  
  
  //typedef std::set<UINT16> eventContainerType;
//   typedef std::multimap<UINT16 , UINT16> eventGroupsContainerType;
//   typedef std::multimap<UINT16 , UINT16> groupPlayersContainerType;
//   typedef std::multimap<UINT16 , UINT16> playerOrderContainerType;
  
//   typedef std::pair<UINT16 , UINT16> pairUINT16Type;
};

#endif // MARKETMAKERSTATE_H
