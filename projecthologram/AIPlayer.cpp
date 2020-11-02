// Fill out your copyright notice in the Description page of Project Settings.


#include "AIPlayer.h"
#include "BaseMovementController.h"
//#include "AIGuardController.h"

#include "Components/SkeletalMeshComponent.h"
#include "Components/InputComponent.h"
#include "Components/CapsuleComponent.h"
#include "Components/StaticMeshComponent.h"

AAIPlayer::AAIPlayer()
{
	UE_LOG(LogTemp, Warning, TEXT("AAIPlayer Constructor called"));
	//fearObject = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("Fear Object"));
	//fearObject->SetupAttachment(RootComponent);
	//fearObject->SetVisibility(false);
	PrimaryActorTick.bCanEverTick = true;
	mentalResistanceMin = 25.0f;
	mentalResistanceMax = 90.0f;
}



void AAIPlayer::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

bool AAIPlayer::TakeMentalDamage(float inputDmg)
{

	if (inputDmg < mentalResistanceMin)
	{
		UE_LOG(LogTemp, Warning, TEXT("%s resisted the attack"), *GetName());
		return false;
	}
	else if (inputDmg >= mentalResistanceMin && inputDmg <= mentalResistanceMax)
	{
		ApplyMentalDamageEffect();

		//AAIGuardController* myController = (AAIGuardController*)Controller;
		//myController->SetState(ATTACKED_EFFECTED);

		UE_LOG(LogTemp, Warning, TEXT("%s is effected by the attack"), *GetName());

		return true;
	}
	else if(inputDmg > mentalResistanceMax)
	{
		ApplyCriticalMentalDamageEffect();

		//AAIGuardController* myController = (AAIGuardController*)Controller;
		//myController->SetState(ATTACK_EFFECTED_CRITICAL);

		UE_LOG(LogTemp, Warning, TEXT("%s is overwhelmed by the attack"), *GetName());

		return true;
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("%s the attack did not register properly!"), *GetName());
		return false;
	}
}

void AAIPlayer::MoveForward(float inputDir)
{
	
}

void AAIPlayer::MoveRight(float inputDir)
{

}

void AAIPlayer::BeginPlay()
{
	//AAIGuardController* myController = (AAIGuardController*)Controller;
	//myController->SetWayPointCount(sizeof(AITargets));
	
}

