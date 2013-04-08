#pragma once
#include "support/support.h"
#include "trash/trashcan.h"


class LeaderBoardEntry : public trashable
{
public:
	LeaderBoardEntry() : m_playerID(0) , m_score(0){}
	~LeaderBoardEntry()
	{
		LOGINFO("Putting LeaderBoardEntry in trash\n");
	}

	UINT16 m_playerID;
	UINT16 m_score;

};

struct compareScoreObj
{
  template<class T>
  bool operator() (const T* obj1 , const T* obj2) const
  {
     return obj1->m_score < obj2->m_score;
  }
};

class Leaderboard : public trashable
{
public:
	
	std::vector<LeaderBoardEntry*> m_players;
	Leaderboard(void){}
	virtual ~Leaderboard(void);

	void AddLeaderBoardEntry(LeaderBoardEntry* t_entry){ m_players.push_back(t_entry);}
	//void RemoveLeaderBoardEntry(LeaderBoardEntry* t_entry){m_players.erase(t_entry);}
	void ClearBoard()
	{
		for(std::vector<LeaderBoardEntry*>::iterator ii = m_players.begin(); ii != m_players.end(); ++ii)
		{
			(*ii)->add_to_trash();
		}
		m_players.clear();
	}
};
