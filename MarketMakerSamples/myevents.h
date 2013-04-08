#ifndef MYEVENTS_H
#define MYEVENTS_H
#include "support/support.h"
#include "trash/trashcan.h"

class MyEvents : public trashable
{

public:
	UINT16 m_eventBalance;
	UINT16 m_moneyInOrders;
	UINT16 m_eventID;
	UINT16 m_grpID;
	UINT16 m_sharePrice;

	MyEvents & operator=( const MyEvents& rhs);
	MyEvents();
	virtual ~MyEvents();

	void pre_trash()
	{
		std::cout << "cleaning up MyEvent" << std::endl;
		m_eventBalance = 0;
		m_eventID = 0;
		m_grpID   = 0;
	}
};

#endif // MYEVENTS_H
