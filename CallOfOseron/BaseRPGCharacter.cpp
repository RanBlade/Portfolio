// Copyright Epic Games, Inc. All Rights Reserved.

#include "BaseRPGCharacter.h"
#include "HeadMountedDisplayFunctionLibrary.h"
#include "Camera/CameraComponent.h"
#include "Components/CapsuleComponent.h"
#include "Components/InputComponent.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "GameFramework/Controller.h"
#include "GameFramework/SpringArmComponent.h"
#include "AbilitySystemComponent.h"

#include "BaseRPGCharacterAttributeSet.h"

//////////////////////////////////////////////////////////////////////////
// ABaseRPGCharacter

ABaseRPGCharacter::ABaseRPGCharacter()
{
	// Set size for collision capsule
	GetCapsuleComponent()->InitCapsuleSize(42.f, 96.0f);

	// set our turn rates for input
	BaseTurnRate = 45.f;
	BaseLookUpRate = 45.f;

	// Don't rotate when the controller rotates. Let that just affect the camera.
	bUseControllerRotationPitch = false;
	bUseControllerRotationYaw = false;
	bUseControllerRotationRoll = false;

	// Configure character movement
	GetCharacterMovement()->bOrientRotationToMovement = true; // Character moves in the direction of input...	
	GetCharacterMovement()->RotationRate = FRotator(0.0f, 540.0f, 0.0f); // ...at this rotation rate
	GetCharacterMovement()->JumpZVelocity = 600.f;
	GetCharacterMovement()->AirControl = 0.2f;

	// Create a camera boom (pulls in towards the player if there is a collision)
	CameraBoom = CreateDefaultSubobject<USpringArmComponent>(TEXT("CameraBoom"));
	CameraBoom->SetupAttachment(RootComponent);
	CameraBoom->TargetArmLength = 300.0f; // The camera follows at this distance behind the character	
	CameraBoom->bUsePawnControlRotation = true; // Rotate the arm based on the controller

	// Create a follow camera
	FollowCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("FollowCamera"));
	FollowCamera->SetupAttachment(CameraBoom, USpringArmComponent::SocketName); // Attach the camera to the end of the boom and let the boom adjust to match the controller orientation
	FollowCamera->bUsePawnControlRotation = false; // Camera does not rotate relative to arm

	// Note: The skeletal mesh and anim blueprint references on the Mesh component (inherited from Character) 
	// are set in the derived blueprint asset named MyCharacter (to avoid direct content references in C++)
	AbilitySystem = CreateDefaultSubobject<UAbilitySystemComponent>(TEXT("AbilitySystem"));
	attributes = CreateDefaultSubobject<UBaseRPGCharacterAttributeSet>(TEXT("Attributes"));

	damagemultiplier = 1.0f;
	damageReductionPercentage = 0.0f;
}

void ABaseRPGCharacter::BeginPlay()
{
	Super::BeginPlay();

	if (AbilitySystem)
	{
		if (RollAbility)
		{
			AbilitySystem->GiveAbility(FGameplayAbilitySpec(RollAbility.GetDefaultObject(),1, 0));
		}
		if (MainHandWeaponAttack)
		{
			AbilitySystem->GiveAbility(FGameplayAbilitySpec(MainHandWeaponAttack.GetDefaultObject(), 1, 0));
		}

		for(int i = 0; i < Abilities.Num(); ++i)
		{
			if(Abilities[i])
			{
				AbilitySystem->GiveAbility(FGameplayAbilitySpec(Abilities[i].GetDefaultObject(), 1, 0));
			}
		}
		UE_LOG(LogTemp, Warning, TEXT("Ability System Active: %s"), *GetName())
		AbilitySystem->InitAbilityActorInfo(this, this);
	}
}

//////////////////////////////////////////////////////////////////////////
// Input

void ABaseRPGCharacter::SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent)
{
	// Set up gameplay key bindings
	check(PlayerInputComponent);
	PlayerInputComponent->BindAction("Jump", IE_Pressed, this, &ACharacter::Jump);
	PlayerInputComponent->BindAction("Jump", IE_Released, this, &ACharacter::StopJumping);

	PlayerInputComponent->BindAxis("MoveForward", this, &ABaseRPGCharacter::MoveForward);
	PlayerInputComponent->BindAxis("MoveRight", this, &ABaseRPGCharacter::MoveRight);

	// We have 2 versions of the rotation bindings to handle different kinds of devices differently
	// "turn" handles devices that provide an absolute delta, such as a mouse.
	// "turnrate" is for devices that we choose to treat as a rate of change, such as an analog joystick
	PlayerInputComponent->BindAxis("Turn", this, &APawn::AddControllerYawInput);
	PlayerInputComponent->BindAxis("TurnRate", this, &ABaseRPGCharacter::TurnAtRate);
	PlayerInputComponent->BindAxis("LookUp", this, &APawn::AddControllerPitchInput);
	PlayerInputComponent->BindAxis("LookUpRate", this, &ABaseRPGCharacter::LookUpAtRate);

	// handle touch devices
	PlayerInputComponent->BindTouch(IE_Pressed, this, &ABaseRPGCharacter::TouchStarted);
	PlayerInputComponent->BindTouch(IE_Released, this, &ABaseRPGCharacter::TouchStopped);

	// VR headset functionality
	PlayerInputComponent->BindAction("ResetVR", IE_Pressed, this, &ABaseRPGCharacter::OnResetVR);

	AbilitySystem->BindAbilityActivationToInputComponent(PlayerInputComponent, FGameplayAbilityInputBinds("ConfirmInput", "CancelInput", "AbilityInput"));
	
}


float ABaseRPGCharacter::GetHealthPercentage()
{
	return attributes->GetHealth() / attributes->GetmaxHealth();
}

float ABaseRPGCharacter::GetHealthCurrent()
{
	return attributes->GetHealth();
}

float ABaseRPGCharacter::GetHealthValueMax()
{
	return attributes->GetmaxHealth();
}

void ABaseRPGCharacter::HandleDamageTemp(float inDamage)
{
	float localHealth = (attributes->GetHealth() - inDamage) * (1.0f - damageReductionPercentage); 

	if (localHealth <= 0.0f)
	{
		localHealth = 0.0f;
	}

	attributes->SetHealth(localHealth);

	OnDamageTaken();

	if (attributes->GetHealth() <= 0.0f)
	{
		OnKilled();
	}
}

void ABaseRPGCharacter::HandleHealingTemp(float inHealth)
{
	float localHealth = attributes->GetHealth() + inHealth;

	if (localHealth >= attributes->GetmaxHealth())
	{
		localHealth = attributes->GetmaxHealth();
	}

	attributes->SetHealth(localHealth);

	OnHealed();
}

void ABaseRPGCharacter::HandleDamage(float inDamage, const FHitResult& HitInfo, const struct FGameplayTagContainer& DamageTags, ABaseRPGCharacter* InstigatorCharacter, AActor* DamageCauser)
{
	OnDamaged(inDamage, HitInfo, DamageTags, InstigatorCharacter, DamageCauser);
}

void ABaseRPGCharacter::SetMaxHealthFromTalent(float inHealth)
{
	float localHealth = attributes->GetmaxHealth() + inHealth;
	attributes->SetmaxHealth(localHealth);
	attributes->SetHealth(localHealth);
}

void ABaseRPGCharacter::SetMaxHealthOnGameLoad(float inHealth)
{
	attributes->SetmaxHealth(inHealth);
	attributes->SetHealth(inHealth);
}

void ABaseRPGCharacter::SetTalentDataOnLoad(float inHealth, float inDamageModifier, float inwalkSpeed, float inRunSpeed)
{
	attributes->SetmaxHealth(inHealth);
	attributes->SetHealth(inHealth);

	damagemultiplier = inDamageModifier;

	
}

void ABaseRPGCharacter::SetCurrentHealthOnLoad(float inHealth)
{
	attributes->SetHealth(inHealth);
}

void ABaseRPGCharacter::OnResetVR()
{
	UHeadMountedDisplayFunctionLibrary::ResetOrientationAndPosition();
}

void ABaseRPGCharacter::TouchStarted(ETouchIndex::Type FingerIndex, FVector Location)
{
		Jump();
}

void ABaseRPGCharacter::TouchStopped(ETouchIndex::Type FingerIndex, FVector Location)
{
		StopJumping();
}

void ABaseRPGCharacter::TurnAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerYawInput(Rate * BaseTurnRate * GetWorld()->GetDeltaSeconds());
}

void ABaseRPGCharacter::LookUpAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerPitchInput(Rate * BaseLookUpRate * GetWorld()->GetDeltaSeconds());
}

void ABaseRPGCharacter::MoveForward(float Value)
{
	if ((Controller != NULL) && (Value != 0.0f))
	{
		// find out which way is forward
		const FRotator Rotation = Controller->GetControlRotation();
		const FRotator YawRotation(0, Rotation.Yaw, 0);

		// get forward vector
		const FVector Direction = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::X);
		AddMovementInput(Direction, Value);
	}
}

void ABaseRPGCharacter::MoveRight(float Value)
{
	if ( (Controller != NULL) && (Value != 0.0f) )
	{
		// find out which way is right
		const FRotator Rotation = Controller->GetControlRotation();
		const FRotator YawRotation(0, Rotation.Yaw, 0);
	
		// get right vector 
		const FVector Direction = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::Y);
		// add movement in that direction
		AddMovementInput(Direction, Value);
	}
}

bool ABaseRPGCharacter::GetCooldownRemainingForTag(FGameplayTagContainer CooldownTags, float& TimeRemaining, float& CooldownDuration)
{
	if (AbilitySystem && CooldownTags.Num() > 0)
	{
		TimeRemaining = 0.f;
		CooldownDuration = 0.f;

		FGameplayEffectQuery const Query = FGameplayEffectQuery::MakeQuery_MatchAnyOwningTags(CooldownTags);
		TArray< TPair<float, float> > DurationAndTimeRemaining = AbilitySystem->GetActiveEffectsTimeRemainingAndDuration(Query);
		if (DurationAndTimeRemaining.Num() > 0)
		{
			int32 BestIdx = 0;
			float LongestTime = DurationAndTimeRemaining[0].Key;
			for (int32 Idx = 1; Idx < DurationAndTimeRemaining.Num(); ++Idx)
			{
				if (DurationAndTimeRemaining[Idx].Key > LongestTime)
				{
					LongestTime = DurationAndTimeRemaining[Idx].Key;
					BestIdx = Idx;
				}
			}

			TimeRemaining = DurationAndTimeRemaining[BestIdx].Key;
			CooldownDuration = DurationAndTimeRemaining[BestIdx].Value;

			return true;
		}
	}

	return false;
}

void ABaseRPGCharacter::InitAttributes(float maxHealth, float inStr, float inDex, float inInt, float inEStr, float inEDex, float inEInt, float inEDamage)
{
	attributes->SetmaxHealth(maxHealth);
	attributes->SetHealth(attributes->GetmaxHealth());

	attributes->SetbaseStrength(inStr);
	attributes->SetbaseIntelligence(inInt);
	attributes->SetbaseDexterity(inDex);

	baseStr = inStr;
	baseInt = inInt;
	baseDex = inDex;
	baseDamage = inEDamage;

	attributes->SetStrength(baseStr + inEStr);
	attributes->SetIntelligence(baseInt + inEInt);
	attributes->SetDexterity(baseDex + inEDex);
}

void ABaseRPGCharacter::IncreaseStatsByPercentage(float inPercentageIncrease)
{
	float localHealth = attributes->GetmaxHealth();
	float localStr = attributes->GetStrength();
	float localDex = attributes->GetDexterity();
	float localInt = attributes->GetIntelligence();

	attributes->SetStrength(localStr * (1 + inPercentageIncrease));
	attributes->SetIntelligence(localInt * (1 + inPercentageIncrease));
	attributes->SetDexterity(localDex * (1 + inPercentageIncrease));
	attributes->SetmaxHealth(localHealth * (1 + inPercentageIncrease));

	attributes->SetHealth(attributes->GetmaxHealth());
}

void ABaseRPGCharacter::UpdateStats(float inStr, float inDex, float inInt, float inDamage)
{
		
	attributes->SetStrength(baseStr + inStr);
	attributes->SetIntelligence(baseInt + inInt);
	attributes->SetDexterity(baseDex + inDex);

	baseDamage = inDamage;
}

float ABaseRPGCharacter::GetEquipmentDamage()
{
	return baseDamage;
}

float ABaseRPGCharacter::GetDamageMultiplier()
{
	return damagemultiplier;
}

void ABaseRPGCharacter::AddToDamageModifier(float infloat)
{
	damagemultiplier += infloat;

	//UE_LOG(LogTemp, Warning, TEXT("Updated damage multipler is %f"), damagemultiplier)
}

float ABaseRPGCharacter::GetDamageReduction()
{
	return damageReductionPercentage;
}

void ABaseRPGCharacter::AddToDamageReduction(float inFloat)
{
	damageReductionPercentage += inFloat;
}
