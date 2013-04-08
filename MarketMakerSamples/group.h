#ifndef _GROUP_H_
#define _GROUP_H_

#include "trash/trashcan.h"
#include "support/support.h"
#include "player.h"
#include "RBHelpers.h"
#include "competitor.h"
#include "log/log.h"
#include "leaderboard.h"

//The Group class is pretty much a data storage class for the Event 
//class. This will act as more of a state class for the EventManager 
//to manage what groups info.
class Group : public trashable
{
public:
  Group()
  {

  }
  ~Group()
  {
    //ClearGroupData();
    LOGINFO("Adding Group to trash\n");
    //add_to_trash();
  }
  void pre_trash()
  {
    ClearGroupData();
    m_groupPlayers.clear();
  }
  void Init(){
    m_groupIDSeed++;
    m_groupID = m_groupIDSeed;
  }
  UINT16 GetID() const { return m_groupID;}
  void SetEventID(UINT16 t_id){m_eventID = t_id;}
  void   AddGroupPlayer( Player* t_user);
  void   RemoveGroupPlayer(Player* t_user){ m_groupPlayers.erase(t_user);}
  UINT16 GetPlayerCount() const { if(m_groupPlayers.empty()){return 0; }else{ return m_groupPlayers.size();}}
  void   AddGroupCompetitor(Competitor* t_competitor){ m_groupComeptitors.insert(t_competitor);}
  
  Competitor* GetCompetitor(UINT16 t_id);
  Player*     GetPlayer(UINT16 t_id);
  
  CompetitorSet GetAllGroupCompetitors() const { return m_groupComeptitors;}
  std::vector<Transaction*> GetAllTransactions() const { return m_transactions;}
  PlayerSet GetAllGroupPlayers() const { return m_groupPlayers;}
  OrderSetT GetSellOrders() const { return m_eventSellorders;}
  OrderSetT GetBuyOrders() const { return m_eventBuyOrders;}
  Order* GetOrder(UINT16 t_id);
  bool RemoveOrder(UINT16);
  bool RemoveOrder(Order* t_order)
  {
    OrderSetT::iterator ii = m_eventBuyOrders.find(t_order);
    OrderSetT::iterator i = m_eventSellorders.find(t_order);
    if(ii != m_eventBuyOrders.end())
    {
      m_eventBuyOrders.erase(ii);
      return true;
    }
    else if(i != m_eventSellorders.end())
    {
      m_eventSellorders.erase(i);
      return true;
    }
    else
      return false;
    
  }
  bool RemoveCompletedOrdersFromGroup()
  {
	  for(OrderSetT::iterator ii = m_completedOrders.begin(); ii != m_completedOrders.end(); ++ii)
	  {
		  OrderSetT::iterator iii = m_eventBuyOrders.find((*ii));

		  if(iii != m_eventBuyOrders.end())
		  {
			m_eventBuyOrders.erase(iii);
		  }
		  else
		  {
			 OrderSetT::iterator i = m_eventSellorders.find((*ii));
			 if(i != m_eventSellorders.end())
			 {
				m_eventSellorders.erase(i);
			 }

		  }

	  }
	  m_completedOrders.clear();
	  return true;
  }
  void AddToRemoveOrder(Order* t_order)
  {
	  m_completedOrders.insert(t_order);
  }

  void AddSellOrder(Order* t_order){ m_eventSellorders.insert(t_order);}
  void AddBuyOrder(Order* t_order){ m_eventBuyOrders.insert(t_order);}
  

  void SetGroupIDForCheck(UINT16 t_id){ m_groupID = t_id;}
  
  void CheckForTransaction();
  void CalculateMoneyLine();
  void SetCompetitorPricePerShare(UINT16, UINT16);
  void ClearGroupData()
  {
    for(OrderSetT::iterator i = m_eventBuyOrders.begin(); i != m_eventBuyOrders.end(); ++i)
    {
      (*i)->add_to_trash();
    }
    for(OrderSetT::iterator i = m_eventSellorders.begin(); i != m_eventSellorders.end(); ++i)
    {
      (*i)->add_to_trash();
    }
    //for(PlayerSet::iterator ii = m_groupPlayers.begin(); ii != m_groupPlayers.end(); ++ii)
    //{
   //   (*ii)->add_to_trash();
   // }
    for(std::vector<Transaction*>::iterator ii = m_transactions.begin(); ii != m_transactions.end(); ++ii)
    {
      (*ii)->add_to_trash();
    }
    
  }
  void AddPlayerToLeaderBoard(UINT16 t_id , UINT16 t_score);
  Leaderboard GetLeaderBoard(){ return m_leaderBoard;}
  void UpdateLeaderBoard();

  void EndEventForGroup(UINT16);

private:
  UINT16                  m_groupID;
  static UINT16           m_groupIDSeed;
  PlayerSet               m_groupPlayers;
  CompetitorSet           m_groupComeptitors;
  Leaderboard             m_leaderBoard;
  
  OrderSetT      m_eventBuyOrders;
  OrderSetT      m_eventSellorders;
  OrderSetT      m_completedOrders;
  std::vector<Transaction*> m_transactions;

  UINT16 m_eventID;

 
};
#endif