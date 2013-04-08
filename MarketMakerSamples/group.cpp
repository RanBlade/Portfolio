#include "group.h"

#include "log/log.h"

UINT16 Group::m_groupIDSeed = 0;

void   Group::AddGroupPlayer( Player* t_user)
{ 
	m_groupPlayers.insert(t_user);
	//UINT16 t_id = t_user->GetID();
	//UINT16 t_score = t_user->GetPlayerEventBalance(m_eventID);
	//AddPlayerToLeaderBoard(t_id , t_score);
}
Competitor* Group::GetCompetitor(UINT16 t_id)
{
	Competitor* t_comp = new Competitor(t_id , "none");

	CompetitorSet::iterator ii = m_groupComeptitors.find(t_comp);
	t_comp->add_to_trash();

	if(ii != m_groupComeptitors.end())
	{
		return (*ii);
	}
	else{
		LOGINFO("Competitor:%i does not exisist in This Group/Event\n" , (*ii)->GetID());
		return NULL;
	}
}

Player* Group::GetPlayer(UINT16 t_id)
{
	Player* t_player = new Player(t_id);
	PlayerSet::iterator ii = m_groupPlayers.find(t_player);


	if(ii != m_groupPlayers.end())
	{
		t_player->add_to_trash();
		return (*ii);
	}
	else{
		LOGINFO("Player:%i does not exisist in this Group:%i\n" , t_player->GetID() , m_groupID);
		t_player->add_to_trash();
		return NULL;
	}
}


Order* Group::GetOrder(UINT16 t_id)
{
	for(OrderSetT::iterator ii = m_eventBuyOrders.begin(); ii != m_eventBuyOrders.end(); ii++)
	{
		if((*ii)->GetID() == t_id)
		{
			return (*ii);
		}
	}
	for(OrderSetT::iterator iii = m_eventSellorders.begin(); iii != m_eventSellorders.end(); iii++)
	{
		if((*iii)->GetID() == t_id)
		{
			return (*iii);
		}
	}
	return NULL;
}
bool Group::RemoveOrder(UINT16 t_id)
{
	for(OrderSetT::iterator ii = m_eventBuyOrders.begin(); ii != m_eventBuyOrders.end(); ii++)
	{
		if((*ii)->GetID() == t_id)
		{
			//(*ii)->add_to_trash();
			m_eventBuyOrders.erase(ii++);
			return true;
		}
	}
	for(OrderSetT::iterator iii = m_eventSellorders.begin(); iii != m_eventSellorders.end(); iii++)
	{
		if((*iii)->GetID() == t_id)
		{
			// (*iii)->add_to_trash();
			m_eventSellorders.erase(iii++);
			return true;
		}
	}
	return false;
}

void Group::CheckForTransaction()
{
	if(m_eventBuyOrders.empty() || m_eventSellorders.empty())
	{
		//LOGINFO("Not enough buy/sell orders to check for transaction\n");
	}
	else
	{
		int x , y;
		x = 0;
		y = 0;
		for(OrderSetT::iterator buyiterator = m_eventBuyOrders.begin(); buyiterator != m_eventBuyOrders.end(); ++buyiterator)
		{
			LOGINFO("GOING TRHOUGHT BUY ORDERS%i\n" , x ); x++;

			for(OrderSetT::iterator selliterator = m_eventSellorders.begin(); selliterator != m_eventSellorders.end(); ++selliterator)
			{
				LOGINFO("GOING THROUGH SELL ORDERS%i\n" , y); y++;
				if((*buyiterator)->GetOwnerID() != (*selliterator)->GetOwnerID())
				{
					LOGINFO("Contracts on Owners don't match\n");
					if((*buyiterator)->GetCompetitorIDForOrder() == (*selliterator)->GetCompetitorIDForOrder())
					{
						LOGINFO("Found match for a competitor in buy/sell\n");
						if((*buyiterator)->GetPriceOfOrder() >= (*selliterator)->GetPriceOfOrder())
						{
							if((*buyiterator)->GetStatus() == ORDER_STATUS_OPEN && (*selliterator)->GetStatus() == ORDER_STATUS_OPEN)
							{
								LOGINFO("FOUND A TRANSACTION! LETS DO IT\n");
								if((*buyiterator)->GetQtyOfOrder() > (*selliterator)->GetQtyOfOrder())
								{
									LOGINFO("BUYER HAS MORE SHARES\n");
									Transaction* t_transaction = new Transaction;
									t_transaction->m_buyerID = (*buyiterator)->GetOwnerID();
									t_transaction->m_sellerID = (*selliterator)->GetOwnerID();
									t_transaction->m_grpID = (*selliterator)->GetGrpID();
									t_transaction->m_eventID = (*selliterator)->GetEventID();
									t_transaction->m_competitorID = (*selliterator)->GetCompetitorIDForOrder();
									t_transaction->m_price = (*buyiterator)->GetPriceOfOrder();
									t_transaction->m_qty = (*selliterator)->GetAmountofContractsInOrder();
									t_transaction->m_buyOrderID = (*buyiterator)->GetID();
									t_transaction->m_sellOrderID = (*selliterator)->GetID();
									t_transaction->m_timeSold = time(0);

									UINT16 t_qty = (*buyiterator)->GetQtyOfOrder() - (*selliterator)->GetQtyOfOrder();
									(*buyiterator)->SetSize(t_qty);
									(*selliterator)->SetStatus(ORDER_STATUS_SOLD);
									AddToRemoveOrder((*selliterator));

									SetCompetitorPricePerShare(t_transaction->m_competitorID , t_transaction->m_price);
									m_transactions.push_back(t_transaction);
								}
								else if((*selliterator)->GetQtyOfOrder() > (*buyiterator)->GetQtyOfOrder())
								{
									LOGINFO("SELLER HAS MORE SHARES\n");
									Transaction* t_transaction = new Transaction;
									t_transaction->m_buyerID = (*buyiterator)->GetOwnerID();
									t_transaction->m_sellerID = (*selliterator)->GetOwnerID();
									t_transaction->m_grpID = (*selliterator)->GetGrpID();
									t_transaction->m_eventID = (*selliterator)->GetEventID();
									t_transaction->m_competitorID = (*selliterator)->GetCompetitorIDForOrder();
									t_transaction->m_price = (*buyiterator)->GetPriceOfOrder();
									t_transaction->m_qty = (*buyiterator)->GetAmountofContractsInOrder();
									t_transaction->m_buyOrderID = (*buyiterator)->GetID();
									t_transaction->m_sellOrderID = (*selliterator)->GetID();
									t_transaction->m_timeSold = time(0);

									UINT16 t_qty = (*selliterator)->GetQtyOfOrder() - (*buyiterator)->GetQtyOfOrder();
									(*selliterator)->SetSize(t_qty);
									(*buyiterator)->SetStatus(ORDER_STATUS_WON);
									AddToRemoveOrder((*buyiterator));

									SetCompetitorPricePerShare(t_transaction->m_competitorID , t_transaction->m_price);
									m_transactions.push_back(t_transaction);
								}
								else if((*selliterator)->GetQtyOfOrder() == (*buyiterator)->GetQtyOfOrder())
								{
									LOGINFO("BUYER/SELLER HAVE EQUAL SHARES\n");
									Transaction* t_transaction = new Transaction;
									t_transaction->m_buyerID = (*buyiterator)->GetOwnerID();
									t_transaction->m_sellerID = (*selliterator)->GetOwnerID();
									t_transaction->m_grpID = (*selliterator)->GetGrpID();
									t_transaction->m_eventID = (*selliterator)->GetEventID();
									t_transaction->m_competitorID = (*selliterator)->GetCompetitorIDForOrder();
									t_transaction->m_price = (*buyiterator)->GetPriceOfOrder();
									t_transaction->m_qty = (*buyiterator)->GetAmountofContractsInOrder();
									t_transaction->m_buyOrderID = (*buyiterator)->GetID();
									t_transaction->m_sellOrderID = (*selliterator)->GetID();
									t_transaction->m_timeSold = time(0);

									LOGINFO("Set Status for both players\n");
									(*selliterator)->SetStatus(ORDER_STATUS_SOLD);
									(*buyiterator)->SetStatus(ORDER_STATUS_WON);
									LOGINFO("Removing Buy Order from event/group order list\n");
									AddToRemoveOrder((*selliterator));
									LOGINFO("Removing SellOrder From Event/group order list\n");
									AddToRemoveOrder((*buyiterator));

									SetCompetitorPricePerShare(t_transaction->m_competitorID , t_transaction->m_price);
									m_transactions.push_back(t_transaction);
								}
							}
						}
					}
				}
			}
		}
	}
	RemoveCompletedOrdersFromGroup();
}




void Group::CalculateMoneyLine()
{

	for(CompetitorSet::iterator i = m_groupComeptitors.begin(); i != m_groupComeptitors.end(); ++i)
	{
		(*i)->SetSharesAtHI(0);
		(*i)->SetSharesAtLO(0);
		int x = 0;// a variable to set the first contract as the testing point and no reset the testing point each time.

		for(OrderSetT::iterator iii = m_eventBuyOrders.begin(); iii != m_eventBuyOrders.end(); ++iii)
		{

			if((*i)->GetID() == (*iii)->GetCompetitorIDForOrder() )
			{
				if(x == 0){ (*i)->SetMarketLo((*iii)->GetPriceOfOrder());}//The test right here for starting point 
				x++;
				if((*i)->GetMarketLineLO() < (*iii)->GetPriceOfOrder())
				{
					(*i)->SetMarketLo( (*iii)->GetPriceOfOrder() );
					(*i)->SetSharesAtLO( (*iii)->GetAmountofContractsInOrder() );
				}
				else if((*i)->GetMarketLineLO() == (*iii)->GetPriceOfOrder())
				{
					UINT16 new_val = (*i)->GetSharesAtLO();
					new_val+=(*iii)->GetAmountofContractsInOrder();
					(*i)->SetSharesAtLO( new_val );
				}
			}
		}
		x = 0;//rest it for the sell orders.
		for(OrderSetT::iterator ii = m_eventSellorders.begin(); ii != m_eventSellorders.end(); ++ii)
		{
			if((*i)->GetID() == (*ii)->GetCompetitorIDForOrder())
			{
				if(x == 0){ (*i)->SetMarketHi((*ii)->GetPriceOfOrder());}//set the first order again.
				x++;
				if((*i)->GetMarketLineHI() > (*ii)->GetPriceOfOrder())
				{
					(*i)->SetMarketHi( (*ii)->GetPriceOfOrder() );
					(*i)->SetSharesAtHI( (*ii)->GetAmountofContractsInOrder() );
				}
				else if((*i)->GetMarketLineHI() == (*ii)->GetPriceOfOrder())
				{
					UINT16 new_val = (*i)->GetsharesatHI();
					new_val+=(*ii)->GetAmountofContractsInOrder();

					(*i)->SetSharesAtHI( new_val );
				}
			} 
		}
		// LOGINFO("MarketLine for Competitor:%i in group:%i is HI:%i | SHARES:%i || LO:%i | SHARES:%i \n" , (*i)->GetID() , m_groupID , (*i)->GetMarketLineHI() ,
		//  (*i)->GetsharesatHI() , (*i)->GetMarketLineLO() , (*i)->GetSharesAtLO());
	}
}

void Group::SetCompetitorPricePerShare(UINT16 t_compID , UINT16 t_price)
{
	Competitor* t_comp = GetCompetitor(t_compID);

	t_comp->SetPrice(t_price);
}

void Group::AddPlayerToLeaderBoard(UINT16 t_id , UINT16 t_score)
{
	LeaderBoardEntry* t_entry = new LeaderBoardEntry;
	t_entry->m_playerID = t_id;
	t_entry->m_score = t_score;

	m_leaderBoard.m_players.push_back(t_entry);
}

bool compareScoreFunc( LeaderBoardEntry* t_obj1 , LeaderBoardEntry* t_obj2)
{
	return (t_obj1->m_score > t_obj2->m_score);
}
void Group::UpdateLeaderBoard()
{
	m_leaderBoard.ClearBoard();

	for(PlayerSet::iterator pp = m_groupPlayers.begin(); pp != m_groupPlayers.end(); ++pp)
	{
		UINT16 t_id = (*pp)->GetID();
		UINT16 t_score = (*pp)->GetPlayerEventBalance(m_eventID);
		LeaderBoardEntry* t_entry = new LeaderBoardEntry;
		t_entry->m_playerID = t_id;
		t_entry->m_score = t_score;
		m_leaderBoard.m_players.push_back(t_entry);
	}
	std::sort(m_leaderBoard.m_players.begin() , m_leaderBoard.m_players.end() , compareScoreFunc);
}

void Group::EndEventForGroup(UINT16 t_winnerID)
{
	for(CompetitorSet::iterator ii = m_groupComeptitors.begin(); ii != m_groupComeptitors.end(); ++ii)
	{
		if((*ii)->GetID() == t_winnerID)
		{
			(*ii)->SetPrice(100);
			(*ii)->SetMarketHi(100);
			(*ii)->SetMarketLo(100);
		}
		else
		{
			(*ii)->SetPrice(0);
			(*ii)->SetMarketHi(0);
			(*ii)->SetMarketLo(0);
		}
	}
}