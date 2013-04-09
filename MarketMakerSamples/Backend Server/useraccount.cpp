#include "useraccount.h"
#include <sstream>
ProcessState UserAccount::m_timeoutState(60);

UserAccount::UserAccount() 
{
  m_player = new Player(m_clientID);
  
}

UserAccount::~UserAccount() 
{
 //m_player.add_to_trash();
}
 UserAccount & UserAccount::operator=(const UserAccount &rhs)
 {
   m_userID = rhs.m_userID;
   m_userName = rhs.m_userName;
   m_passwrd = rhs.m_passwrd;
   m_player = rhs.m_player;
   
   return *this;
   
 }
std::set<UserAccount* , compareAccounts> allAccounts;
std::set<UserAccount* , compareAccounts> ConnectedUserAccounts;


extern void CreateDummyAccounts()
{
  //removed unessasary code block
}

extern UserAccount* GetAccount(std::string t_name)
{
  UserAccount* t_account = new UserAccount;
  t_account->m_userName = t_name;
  std::set<UserAccount* , compareAccounts>::iterator ii = allAccounts.find(t_account);
  if(ii != allAccounts.end()){
    t_account->add_to_trash();
    return (*ii);
  }
  else{
    LOGINFO("Could not find user\n");
    t_account->add_to_trash();;
    return NULL;
  }
  
}
extern UserAccount* GetAccount(UINT16 t_id)
{
  UserAccount* t_account = new UserAccount;
  t_account->m_userID = t_id;
  std::set<UserAccount* , compareAccounts>::iterator ii = allAccounts.find(t_account);
  if(ii != allAccounts.end()){
    t_account->add_to_trash();
    return (*ii);
  }
  else{
    LOGINFO("Could not find user\n");
    t_account->add_to_trash();;
    return NULL;
  }
  
}
extern void SaveAccount(UserAccount* t_account)
{
  UserAccount* saveAccount = GetAccount(t_account->m_userName);
  saveAccount = t_account;
  
  
  std::cout << "accountID: " << saveAccount->GetID() << "  PlayerClientID:" << saveAccount->m_player->GetID() << std::endl;
  
}
extern UserAccount* GetConnectedAccount(UINT16 t_id)
{
  LOGINFO("Attempting to get Connected Client with SessionID:%i\n" , t_id);
  for(std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.begin(); ii != ConnectedUserAccounts.end(); ii++)
  {
    if((*ii)->m_clientID == t_id)
    {
      LOGINFO("Found UserAccount and returning\n");
      return (*ii);
    }
  }
  return NULL;
}
extern void SaveAllAccounts()
{
  for(std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.begin(); ii != ConnectedUserAccounts.end(); ++ii)
  {
    SaveAccount((*ii));
  }
}
extern void DisconnectAllAccounts()
{
  for(std::set<UserAccount* , compareAccounts>::iterator ii = ConnectedUserAccounts.begin(); ii != ConnectedUserAccounts.end(); ++ii)
  {
    (*ii)->add_to_trash();
  }
  ConnectedUserAccounts.clear();
}
