// Fill out your copyright notice in the Description page of Project Settings.


#include "PlayerComponents/InventoryComponent.h"
#include "PlayerComponents/InventoryItemComponent.h"

#include "Containers/Array.h"

FItem emptyItem;
//emptyItem.itemName = "NULL";
// Sets default values for this component's properties
UInventoryComponent::UInventoryComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	emptyItem.itemName = TEXT("EMPTY");
	emptyItem.itemType = TEXT("NONE");
	emptyItem.itemCount = -1;
	emptyItem.tableID = TEXT("Empty");

	inventoryMaxSize = 4;
	CurrentlySelectedItemIndex = 0;

	FItem itemOne, itemTwo, itemThree, itemFour;
	itemOne.itemName = TEXT("EMPTY");
	itemOne.itemType = TEXT("Energy Booster");
	itemOne.itemCount = 1;
	itemOne.tableID = TEXT("Empty");

	itemTwo.itemName = TEXT("EMPTY");
	itemTwo.itemType = TEXT("Key Card");
	itemTwo.itemCount = 1;
	itemTwo.tableID = TEXT("Empty");

	itemThree.itemName = TEXT("EMPTY");
	itemThree.itemType = TEXT("Misc");
	itemThree.itemCount = 1;
	itemThree.tableID = TEXT("Empty");

	itemFour.itemName = TEXT("EMPTY");
	itemFour.itemType = TEXT("Misc");
	itemFour.itemCount = 1;
	itemFour.tableID = TEXT("Empty");

	inventoryItems.Add(itemOne);
	inventoryItems.Add(itemTwo);
	inventoryItems.Add(itemThree);
	inventoryItems.Add(itemFour);
	//inventoryItems = new TArray(inventoryMaxSize);
	// ...
}


void UInventoryComponent::UseSlotOne()
{
	UE_LOG(LogTemp, Warning, TEXT("Slot One used"));

	inventoryItems[0].itemName = "EMPTY";
	inventoryItems[0].tableID = "Empty";
}

void UInventoryComponent::UseSlotTwo()
{
	UE_LOG(LogTemp, Warning, TEXT("Slot Two used"));

	inventoryItems[1].itemName = "EMPTY";
	inventoryItems[1].tableID = "Empty";
}

void UInventoryComponent::UseSlotThree()
{
	UE_LOG(LogTemp, Warning, TEXT("Slot Three used"));

	inventoryItems[2].itemName = "EMPTY";
	inventoryItems[2].tableID = "Empty";
}

void UInventoryComponent::UseSlotFour()
{
	UE_LOG(LogTemp, Warning, TEXT("Slot Four used"));

	inventoryItems[3].itemName = "EMPTY";
	inventoryItems[3].tableID = "Empty";
}

bool UInventoryComponent::CheckIfSlotIsOccupied(int inventoryIndex)
{
	if (inventoryIndex < inventoryMaxSize)
	{
		if (inventoryItems[inventoryIndex].itemName != "EMPTY")
		{
			return true;
		}
		return false;
	}
	return false;
}

// Called when the game starts
void UInventoryComponent::BeginPlay()
{
	Super::BeginPlay();

	// ...
	
}


// Called every frame
void UInventoryComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

bool UInventoryComponent::PickUpItem(FItem newItem)
{
	FString itemType = newItem.itemType;

	UE_LOG(LogTemp, Warning, TEXT("%s itemType to pickup: %s"), *newItem.itemName, *itemType)
	if (itemType == "Energy Booster" && inventoryItems[0].itemName == "EMPTY")
	{
		inventoryItems[0] = newItem;
		return true;
	}
	else if (itemType == "Key Card" && inventoryItems[1].itemName == "EMPTY")
	{
		inventoryItems[1] = newItem;
		return true;
	}
	else if (itemType == "Misc")
	{
		if (inventoryItems[2].itemName == "EMPTY")
		{
			inventoryItems[2] = newItem;
			return true;
		}
		else if (inventoryItems[3].itemName == "EMPTY")
		{
			inventoryItems[3] = newItem;
			return true;
		}
	}
	else
	{
		return false;
	}

	return false;
}

bool UInventoryComponent::InventorySlotOpen()
{
	int inventoryCount = inventoryItems.Num();
	if (inventoryCount < inventoryMaxSize)
	{
		return true;
	}
	else
	{
		return false;
	}
}

void UInventoryComponent::UseItemOnInteract(FString itemName, bool bConsumeItem)
{
	TArray<FItem> removeArray;

	for (int i = 0; i < inventoryItems.Num(); ++i)
	{
		FString elementName = inventoryItems[i].itemName;
		if (elementName == itemName)
		{
			int elementCount = inventoryItems[i].itemCount;
			if (elementCount > 0)
			{
				if (bConsumeItem)
				{
					--inventoryItems[i].itemCount;
				}
			}

			if (bConsumeItem)
			{
				elementCount = inventoryItems[i].itemCount;
				if (elementCount <= 0)
				{
					removeArray.Add(inventoryItems[i]);
					//inventoryItems.Remove(inventoryItems[i]);
				}
			}
		}
	}

	if (removeArray.Num() != 0)
	{
		for (int i = 0; i < removeArray.Num(); ++i)
		{
			inventoryItems.Remove(removeArray[i]);
		}

		removeArray.Empty();
	}
}

void UInventoryComponent::SelectNextItem()
{
	++CurrentlySelectedItemIndex;

	if (CurrentlySelectedItemIndex >= inventoryItems.Num())
	{
		CurrentlySelectedItemIndex = 0;
	}
}

TArray<FItem> UInventoryComponent::GetInventoryArray()
{
	return inventoryItems;
}

