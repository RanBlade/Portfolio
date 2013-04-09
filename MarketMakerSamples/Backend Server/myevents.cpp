#include "myevents.h"
#include "log/log.h"

MyEvents::MyEvents() : m_moneyInOrders(0) , m_eventBalance(0) , m_eventID(0) , m_grpID(0) , m_sharePrice(0)
{

}

MyEvents::~MyEvents()
{
  LOGINFO("Trashing a instance of MyEvents eventID:%i\n" , m_eventID);
}

MyEvents & MyEvents::operator=(const MyEvents& rhs)
{
  m_eventBalance = rhs.m_eventBalance;
  m_eventID = rhs.m_eventID;
  m_grpID = rhs.m_grpID;
  
  return *this;
}
