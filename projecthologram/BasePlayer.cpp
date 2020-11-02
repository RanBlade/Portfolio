// Fill out your copyright notice in the Description page of Project Settings.


#include "BasePlayer.h"
#include "BaseMovementController.h"

#include "Components/InputComponent.h"
#include "Components/SkeletalMeshComponent.h"
#include "Components/CapsuleComponent.h"
#include "Math/Vector.h"

// Sets default values
ABasePlayer::ABasePlayer()
{
	UE_LOG(LogTemp, Warning, TEXT("ABasePlayer Constructor called"));
 	// Set this pawn to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	PlayerMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("Mesh"));
	PlayerCollider = CreateDefaultSubobject<UCapsuleComponent>(TEXT("Collider"));
	
	PlayerCollider->SetCapsuleHalfHeight(100.0f);
	PlayerCollider->SetCapsuleRadius(41.0f);
	PlayerCollider->SetCollisionProfileName(TEXT("PlayerObject"));

	//SetRootComponent(PlayerCollider);
	RootComponent = PlayerCollider;

	PlayerMesh->SetupAttachment(GetRootComponent());
	PlayerMesh->SetRelativeRotation(FRotator(0.0f, -90.0f, 0.0f));
	PlayerMesh->SetRelativeLocation(FVector(0.0f, 0.0f, -100.0f));

	myMovementComponent = CreateDefaultSubobject<UBaseMovementComponent>(TEXT("PlayerMovemntComponent"));
	myMovementComponent->UpdatedComponent = RootComponent;
}

// Called when the game starts or when spawned
void ABasePlayer::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void ABasePlayer::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

// Called to bind functionality to input
void ABasePlayer::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	//InputComponent->BindAxis("MoveForward", this, &ABasePlayer::MoveForward);
	//InputComponent->BindAxis("MoveRight", this, &ABasePlayer::MoveRight);

	//InputComponent->BindAxis("RotatePlayer", this, &ABasePlayer::RotatePlayer);
	//InputComponent->BindAxis("PitchCamera", this, &ABasePlayer::PitchCamera);

	//PlayerInputComponent->BindAction("Jump", IE_Pressed, this, &ABasePlayer::Jump);	
}

UPawnMovementComponent* ABasePlayer::GetMovementComponent() const
{
	return myMovementComponent;
}

void ABasePlayer::MoveForward(float inputDir)
{
	//FVector newDir = GetActorForwardVector() * inputDir;
	//myMovementComponent->AddMovementVector(newDir);

	//UE_LOG(LogTemp, Warning, TEXT("Move Forward pressed"));
}

void ABasePlayer::MoveRight(float inputDir)
{
	//FVector newDir = GetActorRightVector() * inputDir;
	//myMovementComponent->AddMovementVector(newDir);

	//UE_LOG(LogTemp, Warning, TEXT("Move Right pressed"));
}

void ABasePlayer::Jump()
{
	myMovementComponent->StartJump();
	UE_LOG(LogTemp, Warning, TEXT("Move Jump pressed"));
}
