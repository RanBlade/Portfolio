// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "BasePlayer.h"
#include "game.h"
#include "Engine/TargetPoint.h"
#include "AIPlayer.generated.h"

/**
 * 
 */

UCLASS()
class GAME_API AAIPlayer : public ABasePlayer
{
	GENERATED_BODY()

public:
	AAIPlayer();

	// Called every frame
	virtual void Tick(float DeltaTime) override;

	bool TakeMentalDamage(float inDmg);

	UFUNCTION(BlueprintImplementableEvent, Category= "AIPlayer | Damage Effects")
	void ApplyMentalDamageEffect();
	UFUNCTION(BlueprintImplementableEvent, Category = "AIPlayer | Damage Effects")
	void ApplyCriticalMentalDamageEffect();

	virtual void MoveForward(float inputDir) override;
	virtual void MoveRight(float inputDir) override;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
public:

	UPROPERTY(EditAnywhere, Category="Defense Stats", meta = (ClampMin = 0.0f, ClampMax = 50.0f))
	float mentalResistanceMin;
	UPROPERTY(EditAnywhere, Category = "Defense Stats", meta = (ClampMin = 25.0f, ClampMax = 100.0f))
	float mentalResistanceMax;

	//UPROPERTY(EditAnywhere, Category = "Attack Objects")
		//class USkeletalMeshComponent *fearObject;

	//UPROPERTY(EditAnywhere, category = "AI Controller | Navigation")
		//ATargetPoint* AITargets[WAYPOINTCOUNT];

private:



	
};
