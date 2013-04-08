#ifndef ORDER_H
#define ORDER_H
#include "support/support.h"
#include "trash/trashcan.h"
#include <time.h>
#include "log/log.h"


struct compareObjByTime
{
  template<class T>
  bool operator() (const T* obj1 , const T* obj2) const
  {
     return obj1->GetTime() < obj2->GetTime();
  }
};

class Order : public trashable
{

public:
  
  Order(UINT16 t_ownerID , UINT16 t_type, UINT16 t_competitor, UINT16 t_size, UINT16 t_price) : m_OwnerID(t_ownerID) , m_OrderType(t_type) , m_Comeptitor(t_competitor) , m_price(t_price) , m_size(t_size)
  {
    m_timeCreated = time(0);
  }
  Order()
  {
  }
  virtual ~Order()
  {
    LOGINFO("Adding Order to trash\n");
    //add_to_trash();
  }
  void Init()
  {
    m_orderIDseed++;
    m_orderID = m_orderIDseed;
  }
  
  UINT16 GetID() const { return m_orderID; }
  UINT16 GetCompetitorIDForOrder() const { return m_Comeptitor;}
  UINT16 GetPriceOfOrder() const { return m_price;}
  UINT16 GetAmountofContractsInOrder() const { return m_size;}
  UINT16 GetTypeOfOrder() const { return m_OrderType;}
  UINT16 GetQtyOfOrder() const { return m_size;}
  TIME_T GetTime() const { return m_timeCreated;}
  UINT16 GetOwnerID() const { return m_OwnerID;}
  UINT16 GetGrpID() const { return m_grpID;}
  UINT16 GetStatus() const { return m_status;}
  UINT16 GetEventID() const { return m_eventID;}
  
  void SetOwnerID(UINT16 t_id){ m_OwnerID = t_id;}
  void SetOrderType(UINT16 t_type){ m_OrderType = t_type;}
  void SetCompetitor(UINT16 t_id){ m_Comeptitor = t_id;}
  void SetPrice(UINT16 t_price){ m_price = t_price;}
  void SetSize(UINT16 t_size){m_size = t_size;}
  void SetEventID(UINT16 t_id){m_eventID = t_id;}
  void SetGrpID(UINT16 t_id){ m_grpID = t_id;}
  void SetStatus(UINT16 t_status){ m_status = t_status;}
  UINT16 GetTotalCostOfOrder();
  
  
  void  SetIDForChecks(UINT16 t_id){ m_orderID = t_id;}
private:
  UINT16 m_OwnerID;
  UINT16 m_OrderType;  //buy or sell?
  UINT16 m_Comeptitor; //team in the market for which the Order is created. 
  UINT16 m_price;     //price willing to buy/sell for given competitor
  UINT16 m_size;      //amount to be sold/bought at given price
  TIME_T m_timeCreated;
  UINT16 m_eventID;
  UINT16 m_grpID;
  UINT16 m_status;
  UINT16 m_orderID;
  static UINT16 m_orderIDseed;
};

typedef std::set<Order* , compareObjByTime> OrderSetT;
#endif // ORDER_H
