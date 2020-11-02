// Fill out your copyright notice in the Description page of Project Settings.


#include "AIGuardController.h"
#include "AIPlayer.h"


AAIGuardController::AAIGuardController()
{
	if (GetPawn())
	{
		
	}

	SetState(PATROL);
	currentWayPointIndex = 0;
	wayPointCount = WAYPOINTCOUNT;
	
}

void AAIGuardController::Tick(float DeltaTime)
{
	if (GetPawn())
	{
		switch (currentGuardState)
		{
		case PATROL:
		{
			bool playerCheck = CheckForPlayer();
			if (playerCheck)
			{
				SetState(PURSUE);
			}
			else
			{
				bool wayPointCheck = CheckIfAtWayPoint();
				if (wayPointCheck)
				{
					SelectNextWayPoint();
					MoveToWayPoint();
				}
				else
				{					
					UE_LOG(LogTemp, Warning, TEXT("%s traveling to my current way point"), *GetPawn()->GetName());
				}
			}
			break;
		}
		case PURSUE:
		{
			break;
		}
		case IDLE:
		{
			break;
		}
		case ATTACKED_EFFECTED:
		{
			break;
		}
		case ATTACK_EFFECTED_CRITICAL:
		{
			break;
		}
		default:
			break;
		}
	}
}

void AAIGuardController::SetState(EGuardStateSimple newState)
{
	currentGuardState = newState;
}

void AAIGuardController::SetWayPointCount(int newCount)
{
	wayPointCount = newCount;
}

bool AAIGuardController::CheckForPlayer()
{
	return false;
}

void AAIGuardController::SelectNextWayPoint()
{
	currentWayPointIndex++;

	if (currentWayPointIndex > WAYPOINTCOUNT)
	{
		currentWayPointIndex = 0;
	}
}

void AAIGuardController::MoveToWayPoint()
{
	//FVector waypointLoc = ((AAIPlayer*)GetPawn())->AITargets[currentWayPointIndex]->GetActorLocation();

	//UE_LOG(LogTemp, Warning, TEXT("%s switching to my current way point: %s"), *GetPawn()->GetName(), *waypointLoc.ToString());
	//MoveToLocation(waypointLoc);
}

bool AAIGuardController::CheckIfAtWayPoint()
{
	/*FVector pawnLoc = GetPawn()->GetActorLocation();
	FVector waypointLoc = ((AAIPlayer*)GetPawn())->AITargets[currentWayPointIndex]->GetActorLocation();
	FVector distanceToTarget = waypointLoc - pawnLoc;
	UE_LOG(LogTemp, Warning, TEXT("%s distanceToTarget is %f"), *GetPawn()->GetName(), distanceToTarget.Size());
	if (distanceToTarget.Size() < 70.0f)
	{
		UE_LOG(LogTemp, Warning, TEXT("%s has reached the waypoint"), *GetPawn()->GetName());
		return true;
	}
	else
	{
		return false;
	}*/
	return false;
}

bool AAIGuardController::CheckIfTargetIsVisible()
{
	return false;
}

bool AAIGuardController::CheckIfHaveTarget()
{
	return false;
}

bool AAIGuardController::CheckIfTargetInRange()
{
	return false;
}

void AAIGuardController::ChaseTarget()
{
}
