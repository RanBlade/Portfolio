// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Game/Game.h"
#include "InventoryItemComponent.generated.h"


UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class GAME_API UInventoryItemComponent : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UInventoryItemComponent();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	UFUNCTION(BlueprintCallable)
	virtual void SetName(FString newName);
	UFUNCTION(BlueprintCallable)
	FString GetType();
	UFUNCTION(BlueprintCallable)
	int GetCount();
	UFUNCTION(BlueprintCallable)
	FItem GetItem();

public:

protected:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Item Properties")
	FItem item;

		
};
