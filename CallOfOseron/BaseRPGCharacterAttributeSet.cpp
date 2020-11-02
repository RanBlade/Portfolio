// Fill out your copyright notice in the Description page of Project Settings.


#include "BaseRPGCharacterAttributeSet.h"
#include "GameplayEffect.h"
#include "GameplayEffectExtension.h"

#include "NPCBaseRPGCharacter.h"
#include "BaseRPGCharacter.h"

UBaseRPGCharacterAttributeSet::UBaseRPGCharacterAttributeSet()
{
	maxHealth = 200.0f;
	Health = maxHealth;

	Strength = 100.0f;
	Intelligence = 100.0f;
	Dexterity = 100.0f;

	Damage = 1.0f;
}

void UBaseRPGCharacterAttributeSet::PreAttributeChange(const FGameplayAttribute& Attribute, float& NewValue)
{

}

void UBaseRPGCharacterAttributeSet::PostGameplayEffectExecute(const FGameplayEffectModCallbackData& Data)
{
	UE_LOG(LogTemp, Warning, TEXT("PostGameplayEffectExecute: %s"), *Data.Target.AbilityActorInfo->AvatarActor.Get()->GetName())

	AActor* TargetActor	= Data.Target.AbilityActorInfo->AvatarActor.Get();
	ANPCBaseRPGCharacter* NPCCharacter = Cast<ANPCBaseRPGCharacter>(TargetActor);


	UAbilitySystemComponent* Source = Data.EffectSpec.GetContext().GetOriginalInstigatorAbilitySystemComponent();
	AActor* SourceActor = Source->AbilityActorInfo->AvatarActor.Get();

	ABaseRPGCharacter* SourceCharacter = Cast<ABaseRPGCharacter>(SourceActor);

	if(NPCCharacter)
	{
		if (GetDamageAttribute() == Data.EvaluatedData.Attribute)
		{
			if (SourceCharacter)
			{
				float localDamage = 0.0f;
				localDamage = GetDamage() + SourceCharacter->GetEquipmentDamage();
				//UE_LOG(LogTemp, Warning, TEXT("%s: is taking %f damage from: %s"), *Data.Target.AbilityActorInfo->AvatarActor.Get()->GetName(), localDamage * SourceCharacter->GetDamageMultiplier());

				NPCCharacter->HandleDamageTemp(localDamage * SourceCharacter->GetDamageMultiplier());
				SetDamage(0.0f);
			}
			else
			{
				UE_LOG(LogTemp, Warning, TEXT("%s: is taking %f damage"), *Data.Target.AbilityActorInfo->AvatarActor.Get()->GetName(), GetDamage())
				NPCCharacter->HandleDamageTemp(GetDamage());
				SetDamage(0.0f);
			}
		
		}
	}
	else
	{
		ABaseRPGCharacter* PlayerCharcter = Cast<ABaseRPGCharacter>(TargetActor);
		ANPCBaseRPGCharacter* SourceNPC = Cast<ANPCBaseRPGCharacter>(SourceActor);
		if (PlayerCharcter)
		{
			if (GetDamageAttribute() == Data.EvaluatedData.Attribute)
			{
				//UE_LOG(LogTemp, Warning, TEXT("Damage Effect Processing %s"), *Data.Target.AbilityActorInfo->AvatarActor.Get()->GetName())
				UE_LOG(LogTemp, Warning, TEXT("%s: is taking %f damage"), *Data.Target.AbilityActorInfo->AvatarActor.Get()->GetName(), GetDamage())
				PlayerCharcter->HandleDamageTemp(GetDamage() * SourceNPC->GetDamageMultiplier());
				SetDamage(0.0f);

			}
		}
	}
}

void UBaseRPGCharacterAttributeSet::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{

}
