// Fill out your copyright notice in the Description page of Project Settings.


#include "PlayerComponents/Ammo.h"

UAmmo::UAmmo()
{
	item = FItem::FItem();
	item.itemType = TEXT("Energy Booster");
	item.itemCount = 1;
}

void UAmmo::BeginPlay()
{

}

void UAmmo::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{

}
/*
void UAmmo::UseItem()
{
	if (itemCount > 0)
	{
		UE_LOG(LogTemp, Warning, TEXT("Ammo has been used"));
		itemCount--;
	}
}
*/