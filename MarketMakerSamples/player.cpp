#include "player.h"

UINT16 Player::m_playerIDSeed = 0;
Player & Player::operator=(const Player& rhs)
{
  m_playerBalance = rhs.m_playerBalance;
  m_playerEvents  = rhs.m_playerEvents;
  m_playerID = rhs.m_playerID;
  m_playerScore = rhs.m_playerScore;
  m_playerShares = rhs.m_playerShares;
  
  return *this;
}
Shares* Player::CheckForSharesOfCompetitor(UINT16 t_id)
{
  for(std::set<Shares*>::iterator ii = m_playerShares.begin(); ii != m_playerShares.end(); ii++)
  {
    if((*ii)->GetCompetitorID() == t_id)
    {
      return (*ii);
    }
  }
  return NULL;
}

void Player::AddShares(UINT16 t_qty , UINT16 t_compID)
{
 Shares* checkforCompetitor = CheckForSharesOfCompetitor(t_compID);
 
 if(checkforCompetitor != NULL)
 {
   checkforCompetitor->AddToShareQty(t_qty);
 }
 else{
   Shares* t_share = new Shares;
   t_share->AddCompetitor(t_compID);
   t_share->AddToShareQty(t_qty);
   m_playerShares.insert(t_share);
 }
}
void Player::RemoveShares(UINT16 t_qty , UINT16 t_compID)
{
 Shares* checkforCompetitor = CheckForSharesOfCompetitor(t_compID);
 
 if(checkforCompetitor != NULL)
 {
   checkforCompetitor->RemoveFromShareQty(t_qty);
 }
}
void Player::AddBalanceToPlayerEvent(UINT16 t_event, UINT16 t_price)
{
  for(std::set<MyEvents*>::iterator ii = m_playerEvents.begin(); ii != m_playerEvents.end(); ii++)
  {
    if((*ii)->m_eventID == t_event)
    {
      LOGINFO("MODFIED PRICE +%i for event:%i of player%i\n" , t_price , t_event , m_playerID);
      (*ii)->m_eventBalance += t_price;
    }
  }
}
void Player::RemoveBalanceFromPlayerEvent(UINT16 t_event , UINT16 t_price)
{
  for(std::set<MyEvents*>::iterator ii = m_playerEvents.begin(); ii != m_playerEvents.end(); ii++)
  {
    if((*ii)->m_eventID == t_event)
    {
      LOGINFO("MODFIED PRICE -%i for event:%i of player%i\n" , t_price , t_event , m_playerID);
      (*ii)->m_eventBalance -= t_price;
    }
  }
}
UINT16 Player::GetPlayerEventBalance(UINT16 t_id)
{
  for(std::set<MyEvents*>::iterator ii = m_playerEvents.begin(); ii != m_playerEvents.end(); ii++)
  {
    if((*ii)->m_eventID == t_id)
    {
      UINT16 t_shareValue = 0;
	  for(std::set<Shares*>::iterator ss = m_playerShares.begin(); ss != m_playerShares.end(); ++ss)
	  {
		  if((*ss)->GetEventID() == t_id)
			 t_shareValue += (*ss)->GetValueOfAllShares();
	  }
      return (t_shareValue + (*ii)->m_eventBalance);
    }
  }
}
void Player::RemoveOrderFromPlayer(UINT16 t_id)
{
  for(std::vector<Order*>::iterator ii = m_playerOrders.begin(); ii != m_playerOrders.end(); ++ii)
  {
    if(t_id == (*ii)->GetID())
    {
      m_playerOrders.erase(ii);
      return;
    }
  }
}

void Player::SetShareEscrowQty(UINT16 t_compID , UINT16 t_eventID , UINT16 t_qty)
{
 Shares* checkforCompetitor = CheckForSharesOfCompetitor(t_compID);
 
 if(checkforCompetitor != NULL)
 {
	 checkforCompetitor->SetQuanityInOrder(t_qty);
 }
}

void Player::AddToShareEscrowQty(UINT16 t_compID , UINT16 t_eventID , UINT16 t_qty)
{
 Shares* checkforCompetitor = CheckForSharesOfCompetitor(t_compID);
 
 if(checkforCompetitor != NULL)
 {
	 checkforCompetitor->AddToQuanityInOrder(t_qty);
 }
}

void Player::RemoveQtyFromShareEscrow(UINT16 t_compID , UINT16 t_eventID , UINT16 t_qty)
{
 Shares* checkforCompetitor = CheckForSharesOfCompetitor(t_compID);
 
 if(checkforCompetitor != NULL)
 {
	 checkforCompetitor->RemoveQuanityInOrder(t_qty);
 }
}

void Player::AddToBalanceEscrow(UINT16 t_price , UINT16 t_eventID)
{
  for(std::set<MyEvents*>::iterator ii = m_playerEvents.begin(); ii != m_playerEvents.end(); ii++)
  {
    if((*ii)->m_eventID == t_eventID)
    {
      LOGINFO("MODFIED PRICE Escrow +%i for event:%i of player%i\n" , t_price , t_eventID , m_playerID);
	  (*ii)->m_moneyInOrders += t_price;
    }
  }
}

void Player::RemoveFrombalanceEscrow(UINT16 t_price , UINT16 t_eventID)
{
  for(std::set<MyEvents*>::iterator ii = m_playerEvents.begin(); ii != m_playerEvents.end(); ii++)
  {
    if((*ii)->m_eventID == t_eventID)
    {
      LOGINFO("MODFIED PRICE Escrow +%i for event:%i of player%i\n" , t_price , t_eventID , m_playerID);
      (*ii)->m_moneyInOrders += t_price;
    }
  }
}

UINT16 Player::GetPlayerBalanceEscrow(UINT16 t_eventID)
{
  for(std::set<MyEvents*>::iterator ii = m_playerEvents.begin(); ii != m_playerEvents.end(); ii++)
  {
    if((*ii)->m_eventID == t_eventID)
    {
		return (*ii)->m_moneyInOrders;
     // LOGINFO("MODFIED PRICE Escrow +%i for event:%i of player%i\n" , t_price , t_eventID , m_playerID);
      //(*ii)->m_moneyInOrders += t_price;
	}
  }
}

UINT16 Player::GetQtyInEscrowForShare(UINT16 t_compID , UINT16 t_eventID)
{
 Shares* checkforCompetitor = CheckForSharesOfCompetitor(t_compID);
 
 if(checkforCompetitor != NULL)
 {
	 return checkforCompetitor->GetQuanityInOrder();
 }
}
