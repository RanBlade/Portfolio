#ifndef USERACCOUNT_H
#define USERACCOUNT_H

#include "player.h"
#include "process_state.h"

class UserAccount : public trashable
{
private: 
 
public:

  UserAccount();
  virtual ~UserAccount();
  std::string m_passwrd;
  std::string m_userName;
  UINT16      m_userID;
  UINT16      m_clientID;
  UINT16      m_pingchallenge;
  Player*     m_player;
  
  UINT16 GetID() const { return m_userID;}
  
  UserAccount & operator=(const UserAccount &rhs);
  static ProcessState m_timeoutState;
};

struct compareAccounts
{
  template<class T>
  bool operator() (const T* obj1 , const T* obj2) const
  {
     return obj1->m_userName < obj2->m_userName;
  }
};
struct compareAccountsByID
{
  template<class T>
  bool operator() (const T* obj1 , const T* obj2) const
  {
    return obj1->m_clientID < obj2->m_clientID;
  }
};

extern std::set<UserAccount* , compareAccounts> allAccounts;
extern std::set<UserAccount* , compareAccounts> ConnectedUserAccounts;

extern void CreateDummyAccounts();
extern UserAccount* GetAccount(std::string t_name);
extern UserAccount* GetAccount(UINT16);
extern void SaveAccount(UserAccount* t_account);
extern UserAccount* GetConnectedAccount(UINT16 t_id);
extern void SaveAllAccounts();
extern void DisconnectAllAccounts();
#endif // USERACCOUNT_H
