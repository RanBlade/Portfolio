// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Game/Game.h"
#include "Engine/GameInstance.h"
#include "HologramGameInstance.generated.h"

/**
 * 
 */



class AHumanPlayer;

UCLASS()
class GAME_API UHologramGameInstance : public UGameInstance
{
	GENERATED_BODY()

public:
	UHologramGameInstance();

	void UpdatePersistantPlayer(FPlayerPersistentData tData);
	FPlayerPersistentData GetCopyOfPersistentPlayer();

	//UFUNCTION(BlueprintCallable)
	
public:

	FPlayerPersistentData PersistentPlayerData;
	bool bHasPlayerBeenSaved;
	
};
