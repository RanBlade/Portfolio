// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Pawn.h"
#include "BasePlayer.generated.h"


/*USTRUCT()
struct FCameraVariables
{
	GENERATED_USTRUCT_BODY()

	UPROPERTY(VisibleAnywhere)
		float BaseTurnRate;
	UPROPERTY(VisibleAnywhere)
		float BaseLookUpRate;
	UPROPERTY(EditAnywhere)
		float CameraZoomSpeed;
	UPROPERTY(EditAnywhere)
		float CameraMaxZoom;
	UPROPERTY(EditAnywhere)
		float CameraMinZoom;
	UPROPERTY(EditAnywhere)
		float CameraDefaultZoom;
	UPROPERTY(EditAnywhere)
		float VerticalOffSet;
	UPROPERTY(EditAnywhere)
		float PitchOffset;

};*/

UCLASS()
class GAME_API ABasePlayer : public APawn
{
	GENERATED_BODY()

public:
	// Sets default values for this pawn's properties
	ABasePlayer();

	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

	virtual UPawnMovementComponent* GetMovementComponent() const override;

	virtual void MoveForward(float inputDir);
	virtual void MoveRight(float inputDir);
	void Jump();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

//member variables
public:	

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Movement")
	class UBaseMovementComponent* myMovementComponent;

	//UPROPERTY(EditAnywhere, Category = "Camera Variables")
	//FCameraVariables CameraVariables;

	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class USkeletalMeshComponent* PlayerMesh;
	UPROPERTY(EditAnywhere, BlueprintReadOnly)
	class UCapsuleComponent* PlayerCollider;

private:

};
