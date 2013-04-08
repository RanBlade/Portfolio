#include "competitor.h"

UINT16 Competitor::m_competitorIDSeed = 0;

Competitor & Competitor::operator=(const Competitor& t_comp)
{
     m_competitorName = t_comp.m_competitorName;
     m_competitorID   = t_comp.m_competitorID;
     m_marketLineHi   = t_comp.m_marketLineHi;
     m_marketLineLo   = t_comp.m_marketLineLo;
     m_sharesAtHi     = t_comp.m_sharesAtHi;
     m_sharesAtLo     = t_comp.m_sharesAtLo;
	 m_price          = t_comp.m_price;
     
     return *this;
}