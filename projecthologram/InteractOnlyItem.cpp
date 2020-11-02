// Fill out your copyright notice in the Description page of Project Settings.


#include "PlayerComponents/InteractOnlyItem.h"

UInteractOnlyItem::UInteractOnlyItem()
{
	bHasPlayerInteractedWithItem = false;

	item = FItem::FItem();

	item.itemType = "Interactable";
}

void UInteractOnlyItem::BeginPlay()
{

}

void UInteractOnlyItem::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{

}

bool UInteractOnlyItem::HasItemBeenInteractedWith()
{
	return bHasPlayerInteractedWithItem;
}

void UInteractOnlyItem::InteractWithPlayer()
{
	bHasPlayerInteractedWithItem = true;
	ObjectHasBeenInteractedWith();
}
