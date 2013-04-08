#include "order.h"

UINT16 Order::m_orderIDseed = 0;

UINT16 Order::GetTotalCostOfOrder()
{
  return (m_size * m_price);
}