// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "AttributeSet.h"
#include "AbilitySystemComponent.h"
#include "BaseRPGCharacterAttributeSet.generated.h"

/**
 * 
 */

#define ATTRIBUTE_ACCESSORS(ClassName, PropertyName) \
	GAMEPLAYATTRIBUTE_PROPERTY_GETTER(ClassName, PropertyName) \
	GAMEPLAYATTRIBUTE_VALUE_GETTER(PropertyName) \
	GAMEPLAYATTRIBUTE_VALUE_SETTER(PropertyName) \
	GAMEPLAYATTRIBUTE_VALUE_INITTER(PropertyName)



UCLASS()
class BASERPG_API UBaseRPGCharacterAttributeSet : public UAttributeSet
{
	GENERATED_BODY()
	
	public:

	UBaseRPGCharacterAttributeSet();
	
	virtual void PreAttributeChange(const FGameplayAttribute& Attribute, float& NewValue) override;
	virtual void PostGameplayEffectExecute(const FGameplayEffectModCallbackData& Data) override;
	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;
	
	//life support values
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Player Attributes")
	FGameplayAttributeData Health;
	ATTRIBUTE_ACCESSORS(UBaseRPGCharacterAttributeSet, Health)

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Player Attributes")
	FGameplayAttributeData maxHealth;
	ATTRIBUTE_ACCESSORS(UBaseRPGCharacterAttributeSet, maxHealth)


	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Player Attributes")
	FGameplayAttributeData Strength;
	ATTRIBUTE_ACCESSORS(UBaseRPGCharacterAttributeSet, Strength)

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Player Attributes")
	FGameplayAttributeData baseStrength;
	ATTRIBUTE_ACCESSORS(UBaseRPGCharacterAttributeSet, baseStrength)


	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Player Attributes")
	FGameplayAttributeData Intelligence;
	ATTRIBUTE_ACCESSORS(UBaseRPGCharacterAttributeSet, Intelligence)

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Player Attributes")
	FGameplayAttributeData baseIntelligence;
	ATTRIBUTE_ACCESSORS(UBaseRPGCharacterAttributeSet, baseIntelligence)


	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Player Attributes")
	FGameplayAttributeData Dexterity;
	ATTRIBUTE_ACCESSORS(UBaseRPGCharacterAttributeSet, Dexterity)

	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Player Attributes")
	FGameplayAttributeData baseDexterity;
	ATTRIBUTE_ACCESSORS(UBaseRPGCharacterAttributeSet, baseDexterity)


	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Player Attributes")
	FGameplayAttributeData Damage;
	ATTRIBUTE_ACCESSORS(UBaseRPGCharacterAttributeSet, Damage)



};
