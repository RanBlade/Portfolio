#ifndef PLAYER_H
#define PLAYER_H

#include "support/support.h"
#include "trash/trashcan.h"
#include "order.h"
#include "shares.h"
#include "myevents.h"
#include "transaction.h"

class UserAccount;
class Player : public trashable
{

public:
	friend class UserAccount;
	Player(UINT16 t_clientID) : m_playerID(t_clientID)
	{
	}
	virtual ~Player()
	{
		//ClearPlayerData();
		LOGINFO("Adding Player:%i to trash\n" , m_playerID);
		//add_to_trash();
	}
	void pre_trash()
	{
		ClearPlayerData();
	}
	void Init()
	{
		m_playerIDSeed++;
		m_playerID = m_playerIDSeed;
	}

	void SetID(UINT16 t_id){ m_playerID = t_id;}
	void SetBalance(UINT16 t_balance){ m_playerBalance = t_balance;}
	void AddBalanceToPlayerEvent(UINT16 t_event, UINT16 t_price);
	void RemoveBalanceFromPlayerEvent(UINT16 t_event , UINT16 t_price);
	void SetScore(UINT16 t_score){ m_playerScore = t_score;}
	void SetPlayerEvents(std::set<MyEvents*> t_events){ m_playerEvents.insert(t_events.begin() , t_events.end());}
	void AddEventToPlayerList(MyEvents* t_myEvent){ m_playerEvents.insert(t_myEvent);}
	void AddShares(UINT16 t_qty , UINT16 t_compID);
	void AddShares(Shares* t_shares){ m_playerShares.insert(t_shares);}
	void RemoveShares(UINT16 t_qty , UINT16 t_compID); //need to add eventID 
	void SetShareEscrowQty(UINT16 , UINT16 , UINT16);
	void AddToShareEscrowQty(UINT16 t_compID , UINT16 t_eventID , UINT16 t_qty);
	void RemoveQtyFromShareEscrow(UINT16 t_compID , UINT16 t_eventID , UINT16 t_qty);
	void AddTransaction(Transaction* t_trans){ m_transactions.push_back(t_trans); }
	void AddOrderToPlayer(Order* t_order){m_playerOrders.push_back(t_order);}
	void RemoveOrderFromPlayer(UINT16);
	void AddToBalanceEscrow(UINT16 , UINT16);
	void RemoveFrombalanceEscrow(UINT16 , UINT16);
	// void GetShare(UINT16);
	Shares* CheckForSharesOfCompetitor(UINT16 t_id);

	UINT16 GetID() const { return m_playerID; }

	UINT16 GetPlayerScore() const { return m_playerScore;}
	UINT16 GetPlayerBalance() const { return m_playerBalance;}
	UINT16 GetPlayerEventBalance(UINT16 t_id);
	UINT16 GetPlayerBalanceEscrow(UINT16 t_id);
	UINT16 GetQtyInEscrowForShare(UINT16 , UINT16);
	
	Shares* GetShare(UINT16 t_compID) const
	{
		for(std::set<Shares*>::iterator ii = m_playerShares.begin(); ii != m_playerShares.end(); ++ii)
		{
			if((*ii)->GetCompetitorID() == t_compID)
			{
				return (*ii);
			}
		}
		return NULL;
	}
	std::set<Shares*> GetShares() const { return m_playerShares;}
	std::set<MyEvents*> GetPlayerEvents() const { return m_playerEvents;}
	std::vector<Transaction*> GetTransactions() const {return m_transactions;}
	std::vector<Order*> GetPlayerOrders() const { return m_playerOrders;}

	void SetIDForCompare(UINT16 t_id){ m_playerID = t_id;}
	Player & operator=(const Player& rhs);
	void ClearPlayerData()
	{
		for(std::set<MyEvents*>::iterator i = m_playerEvents.begin(); i != m_playerEvents.end(); ++i)
		{
			(*i)->add_to_trash();
		}
		for(std::set<Shares*>::iterator ii = m_playerShares.begin(); ii != m_playerShares.end(); ++ii)
		{
			(*ii)->add_to_trash();
		}
		//for(std::vector<MyEvents*>::iterator iii = m_playerEvents.begin(); iii != m_playerEvents.end(); ++iii)
		// {
		//  (*iii)->add_to_trash();
		//}
		//for(std::vector<>::iterator ii = m_playerEvents.begin(); ii != ; ++ii
	}
private:
	static UINT16    m_playerIDSeed;
	UINT16           m_playerID;  
	std::set<MyEvents*> m_playerEvents;
	UINT16           m_playerScore;
	UINT16           m_playerBalance;  
	std::set<Shares*> m_playerShares;
	std::vector<Transaction*> m_transactions;
	std::vector<Order*> m_playerOrders;



};

#endif // PLAYER_H
