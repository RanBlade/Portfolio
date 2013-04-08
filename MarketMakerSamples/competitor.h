#ifndef COMPETITOR_H
#define COMPETITOR_H

#include "support/support.h"
#include "RBHelpers.h"
#include "trash/trashcan.h"
#include "log/log.h"

class Competitor : public trashable
{

public:
	Competitor(UINT16 t_compID , std::string t_name) : m_competitorID(t_compID) , m_competitorName(t_name) , m_marketLineHi(m_nominalValue) , m_marketLineLo(m_nominalValue) , m_sharesAtHi(0) , m_sharesAtLo(0) , m_price(m_nominalValue)
	{
	}
	Competitor(){}
	Competitor(const Competitor& t_comp)
	{

	}
	virtual ~Competitor()
	{
		m_marketLineHi = 0;
		m_sharesAtHi   = 0;
		m_marketLineLo = 0;
		m_sharesAtLo   = 0;

		LOGINFO("Adding Competitor to trash\n");
		//add_to_trash();
	}

	UINT16 GetID() const { return m_competitorID;}
	std::string GetCompetitorName() const { return m_competitorName;}
	MarketLine GetMarketLine() const
	{
		MarketLine m_pair(m_marketLineHi , m_marketLineLo);

		return m_pair;
	}
	UINT16 GetMarketLineLO() const { return m_marketLineLo;}
	UINT16 GetMarketLineHI() const { return m_marketLineHi;}
	UINT16 GetsharesatHI() const { return m_sharesAtHi;}
	UINT16 GetSharesAtLO() const {return m_sharesAtLo;}
	UINT16 GetPrice() const { return m_price;}

	void SetMarketHi(UINT16 t_hi){ m_marketLineHi = t_hi;}
	void SetMarketLo(UINT16 t_lo){ m_marketLineLo = t_lo;}
	void SetPrice(UINT16 t_price){ m_price = t_price;}

	void AddShareAtLO(){ m_sharesAtLo++;}
	void AddShareAtHI(){ m_sharesAtHi++;}
	void RemoveShareAtLO(){ m_sharesAtLo--;}
	void RemoveShareAtHI(){ m_sharesAtHi--;}

	void SetSharesAtLO(UINT16 t_shares){ m_sharesAtLo = t_shares;}
	void SetSharesAtHI(UINT16 t_shares){ m_sharesAtHi = t_shares;}

	void SetIDForCompare(UINT16 t_id){ m_competitorID = t_id;}

	Competitor & operator=(const Competitor &);
	static const UINT16 m_nominalValue = 50;
private:
	static UINT16 m_competitorIDSeed;
	std::string   m_competitorName;
	UINT16        m_competitorID;
	UINT16        m_marketLineHi;
	UINT16        m_sharesAtHi;
	UINT16        m_marketLineLo;
	UINT16        m_sharesAtLo;

	UINT16        m_price;

};

#endif // COMPETITOR_H
