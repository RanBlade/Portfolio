#include "SportLeagues.h"
#include "dbmanager.h"


CompetitorSet allCompetitors;

extern void PopuplateSportsTeams()
{
	GlobalDataBaseConnection->LoadCompetitorData("mlb" , allCompetitors);
}




extern Competitor* GetSportsTeam(UINT16 t_id)
{
	Competitor* t_competitor = new Competitor;
	t_competitor->SetIDForCompare(t_id);
	CompetitorSet::iterator ii = allCompetitors.find(t_competitor);
	if(ii != allCompetitors.end() )
	{
		t_competitor->add_to_trash();
		return (*ii);
	}
	else{
		LOGINFO("ERROR: Competitor not found with ID:%i\n " , t_id);
		t_competitor->add_to_trash();
		return NULL;
	}
}



extern void CleanupSportsTeams()
{
	for(CompetitorSet::iterator ii = allCompetitors.begin(); ii != allCompetitors.end(); ii++)
	{
		(*ii)->add_to_trash();
	}
}
