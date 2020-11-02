// Copyright Epic Games, Inc. All Rights Reserved.

#include "NPCBaseRPGCharacter.h"
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

ANPCBaseRPGCharacter::ANPCBaseRPGCharacter()
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


	AbilitySystem = CreateDefaultSubobject<UAbilitySystemComponent>(TEXT("AbilitySystem"));
	attributes = CreateDefaultSubobject<UBaseRPGCharacterAttributeSet>(TEXT("Attributes"));

	damagemultiplier = 1.0f;
}

void ANPCBaseRPGCharacter::BeginPlay()
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

	UE_LOG(LogTemp, Warning, TEXT("BeginPlay: %s"), *GetName())
}

//////////////////////////////////////////////////////////////////////////
// Input

void ANPCBaseRPGCharacter::SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent)
{
	// Set up gameplay key bindings
	check(PlayerInputComponent);
	PlayerInputComponent->BindAction("Jump", IE_Pressed, this, &ACharacter::Jump);
	PlayerInputComponent->BindAction("Jump", IE_Released, this, &ACharacter::StopJumping);

	PlayerInputComponent->BindAxis("MoveForward", this, &ANPCBaseRPGCharacter::MoveForward);
	PlayerInputComponent->BindAxis("MoveRight", this, &ANPCBaseRPGCharacter::MoveRight);

	// We have 2 versions of the rotation bindings to handle different kinds of devices differently
	// "turn" handles devices that provide an absolute delta, such as a mouse.
	// "turnrate" is for devices that we choose to treat as a rate of change, such as an analog joystick
	PlayerInputComponent->BindAxis("Turn", this, &APawn::AddControllerYawInput);
	PlayerInputComponent->BindAxis("TurnRate", this, &ANPCBaseRPGCharacter::TurnAtRate);
	PlayerInputComponent->BindAxis("LookUp", this, &APawn::AddControllerPitchInput);
	PlayerInputComponent->BindAxis("LookUpRate", this, &ANPCBaseRPGCharacter::LookUpAtRate);

	// handle touch devices
	PlayerInputComponent->BindTouch(IE_Pressed, this, &ANPCBaseRPGCharacter::TouchStarted);
	PlayerInputComponent->BindTouch(IE_Released, this, &ANPCBaseRPGCharacter::TouchStopped);

	// VR headset functionality
	PlayerInputComponent->BindAction("ResetVR", IE_Pressed, this, &ANPCBaseRPGCharacter::OnResetVR);

	AbilitySystem->BindAbilityActivationToInputComponent(PlayerInputComponent, FGameplayAbilityInputBinds("ConfirmInput", "CancelInput", "AbilityInput"));
	
}


float ANPCBaseRPGCharacter::GetHealthPercentage()
{
	return attributes->GetHealth() / attributes->GetmaxHealth();
}

float ANPCBaseRPGCharacter::GetHealthCurrent()
{
	return attributes->GetHealth();
}

float ANPCBaseRPGCharacter::GetHealthValueMax()
{
	return attributes->GetmaxHealth();
}

void ANPCBaseRPGCharacter::HandleDamageTemp(float inDamage)
{
	attributes->SetHealth(GetHealthCurrent() - inDamage);
	OnDamageTaken();
	if (attributes->GetHealth() <= 0.0f)
	{
		OnKilled();
	}
}

void ANPCBaseRPGCharacter::HandleHealingTemp(float inHealth)
{
	float localHealth = attributes->GetHealth() + inHealth;

	if (localHealth >= attributes->GetmaxHealth())
	{
		localHealth = attributes->GetmaxHealth();
	}

	attributes->SetHealth(localHealth);

	OnHealed();
}

void ANPCBaseRPGCharacter::HandleDamage(float inDamage, const FHitResult& HitInfo, const struct FGameplayTagContainer& DamageTags, ABaseRPGCharacter* InstigatorCharacter, AActor* DamageCauser)
{
	OnDamaged(inDamage, HitInfo, DamageTags, InstigatorCharacter, DamageCauser);
}

void ANPCBaseRPGCharacter::OnResetVR()
{
	UHeadMountedDisplayFunctionLibrary::ResetOrientationAndPosition();
}

void ANPCBaseRPGCharacter::TouchStarted(ETouchIndex::Type FingerIndex, FVector Location)
{
		Jump();
}

void ANPCBaseRPGCharacter::TouchStopped(ETouchIndex::Type FingerIndex, FVector Location)
{
		StopJumping();
}

void ANPCBaseRPGCharacter::TurnAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerYawInput(Rate * BaseTurnRate * GetWorld()->GetDeltaSeconds());
}

void ANPCBaseRPGCharacter::LookUpAtRate(float Rate)
{
	// calculate delta for this frame from the rate information
	AddControllerPitchInput(Rate * BaseLookUpRate * GetWorld()->GetDeltaSeconds());
}

void ANPCBaseRPGCharacter::MoveForward(float Value)
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

void ANPCBaseRPGCharacter::MoveRight(float Value)
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

void ANPCBaseRPGCharacter::InitAttributes(float maxHealth, float inStr, float inDex, float inInt)
{
	attributes->SetmaxHealth(maxHealth);
	attributes->SetHealth(attributes->GetmaxHealth());

	attributes->SetbaseStrength(inStr);
	attributes->SetbaseIntelligence(inInt);
	attributes->SetbaseDexterity(inDex);

	attributes->SetStrength(inStr);
	attributes->SetIntelligence(inInt);
	attributes->SetDexterity(inDex);
}

float ANPCBaseRPGCharacter::GetDamageMultiplier()
{
	return damagemultiplier;
}

void ANPCBaseRPGCharacter::AddToDamageModifier(float infloat)
{
	damagemultiplier += infloat;

	//UE_LOG(LogTemp, Warning, TEXT("Updated damage multipler is %f"), damagemultiplier)
}