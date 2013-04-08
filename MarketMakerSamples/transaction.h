#ifndef TRANSACTION_H
#define TRANSACTION_H
#include "support/support.h"
#include "trash/trashcan.h"

class Transaction : public trashable
{

public:
	Transaction();
	virtual ~Transaction();

	virtual Transaction& operator=(const Transaction& other);


	UINT16 m_price;
	UINT16 m_qty;
	UINT16 m_buyerID;
	UINT16 m_competitorID;
	UINT16 m_sellerID;
	TIME_T m_timeSold;

	UINT16 m_eventID;
	UINT16 m_grpID;
	UINT16 m_sellOrderID;
	UINT16 m_buyOrderID;
	bool m_processed;

};

#endif // TRANSACTION_H
