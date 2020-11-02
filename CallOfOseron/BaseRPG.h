// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"

//#include "BaseRPG.generated.h"

UENUM(BlueprintType)
enum class AbilityInput : uint8
{
	UseAbilityOne UMETA(DisplayName = "Use Slot 1"),
	UseAbilityTwo UMETA(DisplayName = "Use Slot 2"),
	UseAbilityThree UMETA(DisplayName = "Use Slot 3"),
	UseAbilityFour UMETA(DisplayName = "Use Slot 4"),

	UseMainAttack UMETA(DisplayName = "Main Weapon Slot"),
	RollAbility UMETA(DisplayerName = "Roll"),
};
