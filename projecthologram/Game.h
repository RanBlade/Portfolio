// Copyright 1998-2019 Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"

#include "Game.generated.h"



USTRUCT(BlueprintType)
struct FItem
{
	GENERATED_USTRUCT_BODY()

public:
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FString tableID;
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
		FString itemName;
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
		FString itemType;
	UPROPERTY(BlueprintReadWrite, EditAnywhere)
		int itemCount;

	FItem();

	inline bool operator==(const FItem &rhs) const
	{
		return ((itemName == rhs.itemName) &&
			(itemType == rhs.itemType));
	}

	inline bool operator!=(const FItem &rhs) const
	{
		return !((itemName == rhs.itemName) &&
			(itemType == rhs.itemType));

	}

	inline FItem operator=(const FItem &rhs)
	{
		if (this != &rhs)
		{
			itemName = rhs.itemName;
			itemType = rhs.itemType;
			itemCount = rhs.itemCount;
			tableID = rhs.tableID;
		}

		return *this;

	}

};

USTRUCT()
struct FPlayerPersistentData
{
	GENERATED_USTRUCT_BODY()

public:
	UPROPERTY()
		float currentARPool;
	UPROPERTY()
		FItem inventorySlotOne;
	UPROPERTY()
		FItem inventorySlotTwo;
	UPROPERTY()
		FItem inventorySlotThree;
	UPROPERTY()
		FItem inventorySlotFour;

	//FItem();

	inline bool operator==(const FPlayerPersistentData &rhs) const
	{
		return false;
	}

	inline bool operator!=(const FPlayerPersistentData &rhs) const
	{
		return false;

	}

	inline FPlayerPersistentData operator=(const FPlayerPersistentData &rhs)
	{
		currentARPool = rhs.currentARPool;
		inventorySlotOne = rhs.inventorySlotOne;
		inventorySlotTwo = rhs.inventorySlotTwo;
		inventorySlotThree = rhs.inventorySlotThree;
		inventorySlotFour = rhs.inventorySlotFour;


		return *this;

	}

};



#ifndef GAME_H

enum EGuardStateSimple
{
	PATROL,
	PURSUE,
	IDLE,
	ATTACKED_EFFECTED,
	ATTACK_EFFECTED_CRITICAL,
	STATE_COUNT
};



#define WAYPOINTCOUNT 4
#define SPRINTSPEED 700
#define WALKSPEED 300



#endif // !1