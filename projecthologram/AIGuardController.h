// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "game.h"
#include "AIGuardController.generated.h"


/**
 * 
 */

UCLASS()
class GAME_API AAIGuardController : public AAIController
{
	GENERATED_BODY()
	
public:
	AAIGuardController();

	virtual void Tick(float DeltaTime) override;

	//virtual void Posess(APawn *InPawn) override;
	void SetState(EGuardStateSimple newState);
	void SetWayPointCount(int newCount);
private:
	

	//patrol state functions
	bool CheckForPlayer();
	void SelectNextWayPoint();
	void MoveToWayPoint();
	bool CheckIfAtWayPoint();

	//pursue functions
	bool CheckIfTargetIsVisible();
	bool CheckIfHaveTarget();
	bool CheckIfTargetInRange();
	void ChaseTarget();
public:

private:
	EGuardStateSimple currentGuardState;

	class HumanPlayer* myTarget;
	int currentWayPointIndex;
	int wayPointCount;
};
