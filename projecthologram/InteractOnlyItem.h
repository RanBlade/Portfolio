// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PlayerComponents/InventoryItemComponent.h"
#include "InteractOnlyItem.generated.h"

/**
 * 
 */
UCLASS(Blueprintable)
class GAME_API UInteractOnlyItem : public UInventoryItemComponent
{
	GENERATED_BODY()
public:
	UInteractOnlyItem();

	UFUNCTION(BlueprintCallable, BlueprintImplementableEvent, Category = "Interact Actions")
		void ObjectHasBeenInteractedWith();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	//UFUNCTION(BlueprintCallable)
		//virtual void Setup() override;

	UFUNCTION(BlueprintCallable)
	bool HasItemBeenInteractedWith();

	void InteractWithPlayer();
	
public:
protected:
private:

	bool bHasPlayerInteractedWithItem;

};

