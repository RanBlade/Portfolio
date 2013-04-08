#include "shares.h"
#include "log/log.h"


Shares::Shares() : m_competitorID(NO_TEAM) , m_quanity(0) , m_eventID(0) , m_grpID(0) , m_sharePrice(0) , m_quanityInOrder(0)
{
}
Shares::Shares(UINT16 compID , UINT16 t_shareCnt , UINT16 t_price) : m_competitorID(compID) , m_sharePrice(t_price) , m_quanity(t_shareCnt) , m_eventID(0) , m_grpID(0) , m_quanityInOrder(0)
{
}
Shares::~Shares()
{
  LOGINFO("Trashing a Competitors share compID:%i qty:%i and eventID:%i\n" , m_competitorID , m_quanity , m_eventID);
}

Shares & Shares::operator=(const Shares& rhs)
{
  m_competitorID = rhs.m_competitorID;
  m_eventID = rhs.m_eventID;
  m_grpID = rhs.m_grpID;
  m_quanity = rhs.m_quanity;
  
  return *this;
}

