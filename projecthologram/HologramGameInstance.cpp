// Fill out your copyright notice in the Description page of Project Settings.


#include "HologramGameInstance.h"
#include "Game/HumanPlayer.h"

UHologramGameInstance::UHologramGameInstance()
{
	UE_LOG(LogTemp, Warning, TEXT("Hologram GameIstance Created"));
	bHasPlayerBeenSaved = false;
}

void UHologramGameInstance::UpdatePersistantPlayer(FPlayerPersistentData persistantPlayerObj)
{
	UE_LOG(LogTemp, Warning, TEXT("Saving player data"));
	PersistentPlayerData = persistantPlayerObj;

	UE_LOG(LogTemp, Warning, TEXT("Data being Saved: AmmoCount: %f | InventorySlotOneName : %s | InventorySlotOneName : %s | InventorySlotOneName : %s | InventorySlotOneName : %s"), PersistentPlayerData.currentARPool, *PersistentPlayerData.inventorySlotOne.itemName, *PersistentPlayerData.inventorySlotTwo.itemName, *PersistentPlayerData.inventorySlotThree.itemName, *PersistentPlayerData.inventorySlotFour.itemName);

	bHasPlayerBeenSaved = true;
}

FPlayerPersistentData UHologramGameInstance::GetCopyOfPersistentPlayer()
{
	return PersistentPlayerData;
	bHasPlayerBeenSaved = false;
}
