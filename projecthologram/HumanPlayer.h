// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "BasePlayer.h"
#include "Game.h"
#include "HumanPlayer.generated.h"

/**
 * 
 */

class UPawnSoundNoiseEmitterComponent;
class UInventoryComponent;
class UInventoryItemComponent;
class UStaticMeshComponent;
class UDoorLockComponent;


UCLASS()
class GAME_API AHumanPlayer : public ABasePlayer
{
	GENERATED_BODY()
	
public:
	AHumanPlayer();

	virtual void Tick(float DeltaTime) override;
	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

	virtual void MoveForward(float inputDir) override;
	virtual void MoveRight(float inputDir) override;
	

	void SprintPressed();
	void SprintReleased();

	void RotatePlayer(float rotateDir);
	void PitchCamera(float pitchValue);

	void AttackAction();
	void StartDefendAction();
	void StopDefendAction();
	void PickUpAction();
	void UseSlotOne();
	void UseSlotTwo();
	void UseSlotThree();
	void UseSlotFour();

	UFUNCTION(BlueprintCallable)
	void AddARToPool(float inputAmount);
	UFUNCTION(BlueprintCallable)
	float GetARPoolRatio();
	UFUNCTION(BlueprintCallable)
	void AttackEnd();
	UFUNCTION(BlueprintCallable)
	float GetDRPoolRatio();
	UFUNCTION(BlueprintCallable)
	void PlayerCaught();
	UFUNCTION(BlueprintCallable)
	bool GetPlayerCaught();
	UFUNCTION(BlueprintCallable)
	void StopCameraInput();
	UFUNCTION(BlueprintCallable)
	void SetPickUpItem(FItem item, UInventoryItemComponent* pickupComp);
	UFUNCTION(BlueprintCallable)
	void ClearPickUpItem();
	UFUNCTION(BlueprintCallable)
	void SetPickUpObjectStatus(bool newStatus);
	UFUNCTION(BlueprintCallable)
	void AttackDone();
	UFUNCTION(BlueprintCallable)
	bool DoesPlayerHoldAKeyCard();
	UFUNCTION(BlueprintCallable)
	void SetDoorToUnlockBool(UDoorLockComponent* doorComp);
	UFUNCTION(BlueprintCallable)
	void ReportNoise(USoundBase* SoundToPlay, float Volume);

	UFUNCTION(BlueprintCallable)
	void SavePlayerData();
	//UFUNCTION(BlueprintCallable)
	void LoadPlayerData(FPlayerPersistentData tempData);

	UFUNCTION(BlueprintImplementableEvent, Category = "Human Player | Attack Effects")
	void ApplyAttackEffect(float damage, class ACharacter* character);


protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

private:


public:
	UPROPERTY(EditAnywhere, meta = (AllowPrivateAccess = "true"))
	class USpringArmComponent* CameraBoom;
	UPROPERTY(EditAnywhere, meta = (AllowPrivateAccess = "true"))
	class UCameraComponent* FollowCamera;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Human Player | Mutant Abilities | Defense Settings")
	float DRPoolCurrent;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Human Player | Mutant Abilities | Attack Settings")
	float ARPoolCurrent;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Human Player | Camera Settings | Pitch Angles")
	float pitchUpMax;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Human Player | Camera Settings | Pitch Angles")
	float pitchDownMax;
	UPROPERTY(BlueprintReadWrite, VisibleAnywhere, Category = "Human Player | Mutant Abilities | Attack Settings")
	bool isAttacking;

	UPROPERTY(BlueprintReadWrite, Category = "HumanPlayer | Inventory Settings")
	bool bCanuseKeyCard;
	
private:

	FVector2D cameraRotation;
	
	UPROPERTY(BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	bool isDefending;
	UPROPERTY(BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	bool bIsPickingUpObject;
	bool canMove;
	bool isCaught;

	bool bCanAttackAgain;
	float attackCD;
	
	//do once check variables
	bool freezeCameraOnDefendDoOnce;

	UPROPERTY(EditAnywhere, Category = "Human Player | Mutant Abilities | Attack Settings")
	bool bStartARFull;

	FItem pickUpItem;
	UInventoryItemComponent* pickupComponent;
	UDoorLockComponent* doorToUnlock;
	APlayerController* MyController;


	UPROPERTY(EditAnywhere, Category = "Human Player | Mutant Abilities | Defense Settings", meta = (ClampMax = 100.0f, ClampMin = 0.0f))
	float DRPoolMax;
	UPROPERTY(EditAnywhere, Category = "Human Player | Mutant Abilities | Defense Settings", meta = (ClampMax = 25.0f, ClampMin = 10.0f))
	float DRPoolDrainRate;
	UPROPERTY(EditAnywhere, Category = "Human Player | Mutant Abilities | Defense Settings", meta = (ClampMax = 25.0f, ClampMin = 10.0f))
	float DRPoolFillRate;
	UPROPERTY(EditAnywhere, Category = "Human Player | Mutant Abilities | Defense Settings")
	UStaticMeshComponent* defenseiveObjectMesh;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Human Player | Inventory", meta = (AllowPrivateAccess = "true"))
	UInventoryComponent *playerInventory;
	UPROPERTY(VisibleAnywhere)
	UPawnNoiseEmitterComponent* NoiseEmitterComponent;

	UPROPERTY(EditAnywhere, Category = "Human Player | Mutant Abilities | Attack Settings", meta = (ClampMin = 0, ClampMax = 10))
	int ARPoolMax;
	UPROPERTY(EditAnywhere, Category = "Human Player | Mutant Abilities | Attack Settings", meta = (ClampMin = 0, ClampMax = 10000.0f))
	float AttackRange;
	

	
	
};
