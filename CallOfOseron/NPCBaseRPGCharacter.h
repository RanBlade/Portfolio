// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "AbilitySystemComponent.h"
#include "AbilitySystemInterface.h"
#include "GameFramework/Character.h"

#include "BaseRPG.h"

#include "NPCBaseRPGCharacter.generated.h"


class ABaseRPGCharacter;

UCLASS(config=Game)
class ANPCBaseRPGCharacter : public ACharacter, public IAbilitySystemInterface
{
	GENERATED_BODY()

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = Abilities, meta = (AllowPrivateAccess = "true"))
	class UAbilitySystemComponent* AbilitySystem;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Player Attributes", meta = (AllowPrivateAccess = "true"))
	class UBaseRPGCharacterAttributeSet* attributes;

	UAbilitySystemComponent* GetAbilitySystemComponent() const override 
	{
		return AbilitySystem;
	};

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Abilities", meta = (AllowPrivateAccess = "true"))
	TSubclassOf<class UGameplayAbility> RollAbility;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Abilities", meta = (AllowPrivateAccess = "true"))
	TSubclassOf<class UGameplayAbility> MainHandWeaponAttack;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Abilities", meta = (AllowPrivateAccess = "true"))
	TArray<TSubclassOf<class UGameplayAbility>> Abilities;

public:
	ANPCBaseRPGCharacter();

	/** Base turn rate, in deg/sec. Other scaling may affect final turn rate. */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category=Camera)
	float BaseTurnRate;

	/** Base look up/down rate, in deg/sec. Other scaling may affect final rate. */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category=Camera)
	float BaseLookUpRate;

	void BeginPlay() override;

protected:

	/** Resets HMD orientation in VR. */
	void OnResetVR();

	/** Called for forwards/backward input */
	void MoveForward(float Value);

	/** Called for side to side input */
	void MoveRight(float Value);

	/** 
	 * Called via input to turn at a given rate. 
	 * @param Rate	This is a normalized rate, i.e. 1.0 means 100% of desired turn rate
	 */
	void TurnAtRate(float Rate);

	/**
	 * Called via input to turn look up/down at a given rate. 
	 * @param Rate	This is a normalized rate, i.e. 1.0 means 100% of desired turn rate
	 */
	void LookUpAtRate(float Rate);

	/** Handler for when a touch input begins. */
	void TouchStarted(ETouchIndex::Type FingerIndex, FVector Location);

	/** Handler for when a touch input stops. */
	void TouchStopped(ETouchIndex::Type FingerIndex, FVector Location);

protected:
	// APawn interface
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;
	// End of APawn interface

public:

	UFUNCTION(BlueprintCallable, Category = "Attribute Helper Functions")
	float GetHealthPercentage();
	UFUNCTION(BlueprintCallable, Category = "Attribute Helper Functions")
	float GetHealthCurrent();
	UFUNCTION(BlueprintCallable, Category = "Attribute Helper Functions")
	float GetHealthValueMax();

	UFUNCTION(BlueprintCallable)
	void HandleDamageTemp(float inDamage);
	void HandleDamage(float inDamage, const FHitResult& HitInfo, const struct FGameplayTagContainer& DamageTags, ABaseRPGCharacter* InstigatorCharacter, AActor* DamageCauser);

	UFUNCTION(BlueprintImplementableEvent)
	void OnDamaged(float DamageAmount, const FHitResult& HitInfo, const struct FGameplayTagContainer& DamageTags, ABaseRPGCharacter* InstigatorCharacter, AActor* DamageCauser);
	
	UFUNCTION(BlueprintCallable)
	void HandleHealingTemp(float inHealth);

	UFUNCTION(BlueprintImplementableEvent)
	void OnKilled();
	UFUNCTION(BlueprintImplementableEvent)
	void OnDamageTaken();
	UFUNCTION(BlueprintImplementableEvent)
	void OnHealed();

	UFUNCTION(BlueprintCallable)
	void InitAttributes(float maxHealth, float inStr, float inDex, float inInt);

	UFUNCTION()
		float GetDamageMultiplier();
	UFUNCTION(BlueprintCallable)
		void AddToDamageModifier(float infloat);

private:  
	
	float damagemultiplier;
};

