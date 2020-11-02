// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PawnMovementComponent.h"
#include "BaseMovementController.generated.h"

/**
 * 
 */
UCLASS()
class GAME_API UBaseMovementComponent : public UPawnMovementComponent
{
	GENERATED_BODY()
public:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Player Movement | Movement")
	float walkSpeed;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Player Movement | Movement")
	float runSpeed;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Player Movement | Movement")
	float currentSpeed;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Player Movement | Jump")
	float maxJumpHeight;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Player Movement | Jump")
	float jumpSpeed;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite)
	bool isJumping;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	bool isCurrentlyFalling;

private:
	FVector movementInput;
	FVector jumpStartVec;

	float verticalVelocityLastTick;

	float baseMovementSpeed;
	bool bIsAllowedToMove;

public:

	UBaseMovementComponent();
	virtual void TickComponent(float DeltaTime, enum ELevelTick TickType, FActorComponentTickFunction *ThisTickFunction) override;
	virtual void BeginPlay() override;
	void StartJump();

	void AddMovementVector(FVector input);

	void SetMovementSpeed(float inSpeed);
	void SetRunSpeedActive();
	void SetWalkSpeedActive();
	void StopAllMovement();
	void EnableAllMovement();

private:

	bool CheckIfonGround();
};
