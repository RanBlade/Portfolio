// Fill out your copyright notice in the Description page of Project Settings.


#include "PlayerComponents/KeyCard.h"

UKeyCard::UKeyCard()
{
	item = FItem::FItem();
	item.itemType = TEXT("Key Card");
	item.itemCount = 1;
}

void UKeyCard::BeginPlay()
{

}

void UKeyCard::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{

}
/*
void UKeyCard::UseItem()
{

}
*/