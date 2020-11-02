// Fill out your copyright notice in the Description page of Project Settings.


#include "PlayerComponents/InventoryItemComponent.h"

// Sets default values for this component's properties
UInventoryItemComponent::UInventoryItemComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
	//item = new FItem();
}


// Called when the game starts
void UInventoryItemComponent::BeginPlay()
{
	Super::BeginPlay();

	// ...
	
}


// Called every frame
void UInventoryItemComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

void UInventoryItemComponent::SetName(FString newName)
{
	item.itemName = newName;

}

FString UInventoryItemComponent::GetType()
{
	//return itemType;
	return item.itemType;
}

int UInventoryItemComponent::GetCount()
{
	//return itemCount;
	return item.itemCount;
}

FItem UInventoryItemComponent::GetItem()
{
	return item;
}

