#ifndef _SPORTS_LEAGUES_H_
#define _SPORTS_LEAGUES_H_

#include "RBHelpers.h"
#include "competitor.h"
#include "log/log.h"

extern CompetitorSet allCompetitors;

    //A function called to populate a list of competitors 
    extern void PopuplateSportsTeams();
  extern Competitor* GetSportsTeam(UINT16 t_id);
  extern void CleanupSportsTeams();

#endif