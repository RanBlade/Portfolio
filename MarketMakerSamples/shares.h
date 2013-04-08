#ifndef SHARES_H
#define SHARES_H
#include "support/support.h"
#include "trash/trashcan.h"
#include "RBHelpers.h"

class Shares : public trashable
{

public:
	Shares();
	Shares(UINT16 compID , UINT16 t_shareCnt , UINT16 t_price);
	virtual ~Shares();

	UINT16 GetShareCount()const {return m_quanity;}
	UINT16 GetGrpID() const { return m_grpID;}
	UINT16 GetEventID() const { return m_eventID;}
	UINT16 GetCompetitorID() const { return m_competitorID;}
	UINT16 GetSharePrice() const {return m_sharePrice;}
	UINT16 GetValueOfAllShares() const {return (m_quanity * m_sharePrice);}
	UINT16 GetQuanityInOrder() const {return m_quanityInOrder;}

	void SetGrpID(UINT16 t_id){ m_grpID = t_id;}
	void SetEventID(UINT16 t_id){ m_eventID = t_id;}
	void SetShareQty(UINT16 t_qty){ m_quanity = t_qty;}
	void AddCompetitor(UINT16 t_id){ m_competitorID = t_id;}
	void AddToShareQty(UINT16 t_qty) { m_quanity += t_qty;}
	void RemoveFromShareQty(UINT16 t_qty){ m_quanity-=t_qty;}
	void SetSharePrice(UINT16 t_price){m_sharePrice = t_price;}
	void SetQuanityInOrder(UINT16 t_qty){m_quanityInOrder = t_qty;}
	void AddToQuanityInOrder(UINT16 t_qty){m_quanityInOrder += t_qty;}
	void RemoveQuanityInOrder(UINT16 t_qty){m_quanityInOrder -= t_qty;}
	

	Shares & operator=(const Shares& rhs);
private:
	UINT16 m_competitorID;
	UINT16 m_quanity;
	UINT16 m_quanityInOrder;
	UINT16 m_grpID;
	UINT16 m_eventID;

	UINT16 m_sharePrice;
};

#endif // SHARES_H
