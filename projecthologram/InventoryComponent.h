// Fill out your copyright notice in the Description page of Project Settings.

#pragma once


#include "CoreMinimal.h"
#include "Game/Game.h"
#include "Components/ActorComponent.h"
#include "InventoryComponent.generated.h"

/*USTRUCT(BlueprintType)
struct FItem
{
	GENERATED_USTRUCT_BODY()

public:
	UPROPERTY(BlueprintReadOnly, EditAnywhere)
	FString itemName;
	UPROPERTY(BlueprintReadOnly, EditAnywhere)
	FString itemType;
	UPROPERTY(BlueprintReadOnly, EditAnywhere)
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
		}

		return *this;

	}
	
};*/

#ifndef _EMPTY_ITEM
#define _EMPTY_ITEM

extern FItem emptyItem;

#endif
//emptyItem->itemCount = -1;
//emptyItem->itemName = "NULL";
//emptyItem->itemType = "NULL";


class UInventoryItemComponent;
//struct FItem;

UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class GAME_API UInventoryComponent : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UInventoryComponent();

	void UseSlotOne();
	void UseSlotTwo();
	void UseSlotThree();
	void UseSlotFour();

	bool CheckIfSlotIsOccupied(int inventoryIndex);


protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	UFUNCTION(BlueprintCallable)
	bool PickUpItem(FItem newItem);
	UFUNCTION(BlueprintCallable)
	bool InventorySlotOpen();
	UFUNCTION(BlueprintCallable)
	void UseItemOnInteract(FString itemName, bool bConsumeItem = true);
	UFUNCTION(BlueprintCallable)
	void SelectNextItem();
	UFUNCTION(BlueprintCallable)
	TArray<FItem> GetInventoryArray();

public:

private:
	int inventoryMaxSize;
	int CurrentlySelectedItemIndex;
	TArray<FItem> inventoryItems;
		
};
