#include "transaction.h"

Transaction::Transaction() : m_processed(false)
{

}

Transaction::~Transaction()
{

}

Transaction& Transaction::operator=(const Transaction& other)
{
return *this;
}

