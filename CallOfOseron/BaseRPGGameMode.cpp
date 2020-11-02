// Copyright Epic Games, Inc. All Rights Reserved.

#include "BaseRPGGameMode.h"
#include "BaseRPGCharacter.h"
#include "UObject/ConstructorHelpers.h"

ABaseRPGGameMode::ABaseRPGGameMode()
{
	// set default pawn class to our Blueprinted character
	//static ConstructorHelpers::FClassFinder<APawn> PlayerPawnBPClass(TEXT("/Game/ThirdPersonCPP/Blueprints/ThirdPersonCharacter"));
	//if (PlayerPawnBPClass.Class != NULL)
	//{
	//	DefaultPawnClass = PlayerPawnBPClass.Class;
	//}
}
