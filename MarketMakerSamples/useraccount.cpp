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
  UserAccount* t_account = new UserAccount;
  t_account->m_passwrd = "greta";
  t_account->m_userName = "TesterEric";
  t_account->m_userID   = 750;
  t_account->m_player->SetBalance(10000);
  t_account->m_player->SetScore(1000);
  allAccounts.insert(t_account);
  
  UserAccount* t_account1 = new UserAccount;
  t_account1->m_passwrd = "berkeley";
  t_account1->m_userName = "TesterJames";
  t_account1->m_userID   = 990;
  t_account1->m_player->SetBalance(10000);
  t_account1->m_player->SetScore(1000);
  allAccounts.insert(t_account1);
  
  UserAccount* t_account2 = new UserAccount;
  t_account2->m_passwrd = "trader1";
  t_account2->m_userName = "TesterRace";
  t_account2->m_userID   = 300;
  t_account2->m_player->SetBalance(10000);
  t_account2->m_player->SetScore(1000);
  allAccounts.insert(t_account2);
  
    UserAccount* t_account3 = new UserAccount;
  t_account3->m_passwrd = "trader44";
  t_account3->m_userName = "TesterRae";
  t_account3->m_userID   = 400;
  t_account3->m_player->SetBalance(10000);
  t_account3->m_player->SetScore(1000);
  allAccounts.insert(t_account3);
  
    UserAccount* t_account4 = new UserAccount;
  t_account4->m_passwrd = "trader55";
  t_account4->m_userName = "TesterRicky";
  t_account4->m_userID   = 600;
  t_account4->m_player->SetBalance(10000);
  t_account4->m_player->SetScore(1000);
  allAccounts.insert(t_account4);
  
    UserAccount* t_account5 = new UserAccount;
  t_account5->m_passwrd = "trader66";
  t_account5->m_userName = "TesterJohn";
  t_account5->m_userID   = 500;
  t_account5->m_player->SetBalance(10000);
  t_account5->m_player->SetScore(1000);
  allAccounts.insert(t_account5);
  
  for(int i = 0 , y = 5; i < 40 ; i++ , y++)
  {
    UserAccount* t_account12 = new UserAccount;
	std::stringstream t_uname , t_pwd;
	t_uname << "cojones" << i;
	t_pwd << "tester" << y;
    t_account12->m_passwrd = t_pwd.str();
    t_account12->m_userName = t_uname.str() ;
    t_account12->m_userID   = y;
    t_account12->m_player->SetBalance(10000);
    t_account12->m_player->SetScore(1000);
    allAccounts.insert(t_account12);
   // y++;
  }
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
