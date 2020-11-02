// Fill out your copyright notice in the Description page of Project Settings.


#include "HumanPlayer.h"
#include "BaseMovementController.h"
#include "AIPlayer.h"
#include "PlayerComponents/InventoryComponent.h"
#include "PlayerComponents/InventoryItemComponent.h"
#include "PlayerComponents/InteractOnlyItem.h"
#include "DoorLockComponent.h"
#include "HologramGameInstance.h"

#include "Engine.h"
#include "GameFramework/SpringArmComponent.h"
#include "Camera/CameraComponent.h"
#include "Components/InputComponent.h"
#include "Components/CapsuleComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Perception/PawnSensingComponent.h"
#include "Kismet/GameplayStatics.h"

AHumanPlayer::AHumanPlayer()
{
	UE_LOG(LogTemp, Warning, TEXT("AHumanPlayer Constructor called"));

		CameraBoom = CreateDefaultSubobject<USpringArmComponent>(TEXT("CameraBoom"));
		FollowCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("FollowCamera"));
		defenseiveObjectMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Defensive Object"));
		playerInventory = CreateDefaultSubobject<UInventoryComponent>(TEXT("Inventory"));
		NoiseEmitterComponent = CreateDefaultSubobject<UPawnNoiseEmitterComponent>(TEXT("Noise Emitter"));
		defenseiveObjectMesh->SetupAttachment(RootComponent);
		defenseiveObjectMesh->SetVisibility(false);

		//CameraVariables.CameraMaxZoom = 700.0f;
		//CameraVariables.CameraMinZoom = 100.0f;
		//CameraVariables.CameraDefaultZoom = 450.0f;
		//CameraVariables.CameraZoomSpeed = 25.0f;

		//CameraVariables.PitchOffset = 345.0f;
		//CameraVariables.VerticalOffSet = 100.0f;

		CameraBoom->SetupAttachment(GetRootComponent());
		CameraBoom->TargetArmLength = 450.0f;
		CameraBoom->bUsePawnControlRotation = false;

		FollowCamera->SetupAttachment(CameraBoom, USpringArmComponent::SocketName);
		FollowCamera->bUsePawnControlRotation = false;

		//CameraVariables.BaseTurnRate = 65.0f;
		//CameraVariables.BaseLookUpRate = 65.0f;

		CameraBoom->SetRelativeLocation(FVector(0.0f, 0.0f, 100.0f));
		CameraBoom->SetRelativeRotation(FRotator(345.0f, 0.0f, 0.0f));

		isDefending = false;
		canMove = true;
		isCaught = false;

		DRPoolDrainRate = 25.0f;
		DRPoolFillRate = 10.0f;
		DRPoolMax = 100.0f;
		DRPoolCurrent = DRPoolMax;

		ARPoolMax = 10;
		ARPoolCurrent = ARPoolMax;
		AttackRange = 1000.0f;

		freezeCameraOnDefendDoOnce = false;

		pitchDownMax = 20.0f;
		pitchUpMax = -80.0f;

		pickUpItem = emptyItem;
		bCanuseKeyCard = false;

		bStartARFull = false;

		bCanAttackAgain = true;
		attackCD = 0.0f;
		MyController = UGameplayStatics::GetPlayerController(this, 0);
}


void AHumanPlayer::BeginPlay()
{
	Super::BeginPlay();

	if (bStartARFull)
	{
		ARPoolCurrent = ARPoolMax;
	}
	else
	{
		ARPoolCurrent = 0;
	}

	if (GetWorld()->GetGameInstance<UHologramGameInstance>()->bHasPlayerBeenSaved)
	{
		UE_LOG(LogTemp, Warning, TEXT("Loading Player Data"));
		FPlayerPersistentData data = GetWorld()->GetGameInstance<UHologramGameInstance>()->GetCopyOfPersistentPlayer();	
		LoadPlayerData(data);
	}

}

void AHumanPlayer::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	FRotator newRotation = GetActorRotation();
	newRotation.Yaw += cameraRotation.X;
	SetActorRotation(newRotation);

	FRotator NewCameraPitch = CameraBoom->GetComponentRotation();
	NewCameraPitch.Pitch = FMath::Clamp(NewCameraPitch.Pitch += cameraRotation.Y, pitchUpMax, pitchDownMax);
	CameraBoom->SetWorldRotation(NewCameraPitch);
	//UE_LOG(LogTemp, Warning, TEXT("Current velocity: %f"), myMovementComponent->Velocity.Size());

	if (isDefending)
	{
		DRPoolCurrent -= (DRPoolDrainRate * DeltaTime);

		if (DRPoolCurrent <= 0.0f)
		{
			StopDefendAction();
		}
	}
	else
	{
		if (DRPoolCurrent < DRPoolMax)
		{
			DRPoolCurrent += (DRPoolFillRate * DeltaTime);
		}
	}

	if (!bCanAttackAgain)
	{
		attackCD += DeltaTime;

		if (attackCD > 0.91f)
		{
			bCanAttackAgain = true;
			this->EnableInput(MyController);
			isAttacking = false;
		}
	}
}

void AHumanPlayer::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	InputComponent->BindAxis("MoveForward", this, &AHumanPlayer::MoveForward);
	InputComponent->BindAxis("MoveRight", this, &AHumanPlayer::MoveRight);

	InputComponent->BindAxis("RotatePlayer", this, &AHumanPlayer::RotatePlayer);
	InputComponent->BindAxis("PitchCamera", this, &AHumanPlayer::PitchCamera);

	InputComponent->BindAction("Jump", IE_Pressed, this, &ABasePlayer::Jump);
	InputComponent->BindAction("Attack", IE_Pressed, this, &AHumanPlayer::AttackAction);
	InputComponent->BindAction("PickUp", IE_Pressed, this, &AHumanPlayer::PickUpAction);

	InputComponent->BindAction("invSlotOne", IE_Pressed, this, &AHumanPlayer::UseSlotOne);
	InputComponent->BindAction("invSlotTwo", IE_Pressed, this, &AHumanPlayer::UseSlotTwo);
	InputComponent->BindAction("invSlotThree", IE_Pressed, this, &AHumanPlayer::UseSlotThree);
	InputComponent->BindAction("invSlotFour", IE_Pressed, this, &AHumanPlayer::UseSlotFour);

	InputComponent->BindAction("Defend", IE_Pressed, this, &AHumanPlayer::StartDefendAction);
	InputComponent->BindAction("Defend", IE_Released, this, &AHumanPlayer::StopDefendAction);

	InputComponent->BindAction("Sprint", IE_Pressed, this, &AHumanPlayer::SprintPressed);
	InputComponent->BindAction("Sprint", IE_Released, this, &AHumanPlayer::SprintReleased);
}

void AHumanPlayer::MoveForward(float inputDir)
{
	if (canMove && !isCaught)
	{
		FVector newDir = GetActorForwardVector() * inputDir;
		myMovementComponent->AddMovementVector(newDir);
	}

	//UE_LOG(LogTemp, Warning, TEXT("Move Forward pressed"));
}

void AHumanPlayer::MoveRight(float inputDir)
{
	if (canMove && !isCaught)
	{
		FVector newDir = GetActorRightVector() * inputDir;
		myMovementComponent->AddMovementVector(newDir);
	}

	//UE_LOG(LogTemp, Warning, TEXT("Move Right pressed"));
}

void AHumanPlayer::SprintPressed()
{
	myMovementComponent->SetRunSpeedActive();
}

void AHumanPlayer::SprintReleased()
{
	myMovementComponent->SetWalkSpeedActive();
}

void AHumanPlayer::RotatePlayer(float rotateDir)
{
	if (canMove && !isCaught)
	{
		cameraRotation.X = rotateDir;
	}
}

void AHumanPlayer::PitchCamera(float pitchValue)
{
	if (!isCaught)
	{
		cameraRotation.Y = pitchValue;
	}
}

void AHumanPlayer::AttackAction()
{
	if (ARPoolCurrent > 0.0f && bCanAttackAgain)
	{
		

		TArray<FHitResult> attackHit;
		float colliderOffset = PlayerCollider->GetUnscaledCapsuleRadius() + 5.0f;
		FVector startTrace = GetActorLocation() + (GetActorForwardVector() * colliderOffset);
		FVector directionVec = GetActorForwardVector();
		FVector endTrace = ((directionVec * AttackRange) + startTrace);
		FQuat ActorRot = GetActorRotation().Quaternion();

		FCollisionShape attackShape;


		FCollisionQueryParams *TraceParams = new FCollisionQueryParams();

		if (GetWorld()->SweepMultiByChannel(attackHit, startTrace, endTrace, ActorRot, ECC_GameTraceChannel2, FCollisionShape::MakeSphere(200.0f),*TraceParams))
		{
			for (int i = 0; i < attackHit.Num(); ++i)
			{
				UE_LOG(LogTemp, Warning, TEXT("attackHit name: %s with tag: %s"), *attackHit[i].GetActor()->GetName(), attackHit[i].GetActor()->ActorHasTag("Enemy") ? TEXT("TRUE") : TEXT("FALSE"));
				//DrawDebugSphere(GetWorld(), );
				//DrawDebugLine(GetWorld(), startTrace, endTrace, FColor(255, 0.0f, 255), true);
				if (attackHit[i].GetActor()->ActorHasTag("Enemy"))
				{
					//ApplyAttackEffect();
					//DrawDebugLine(GetWorld(), startTrace, endTrace, FColor(255, 0.0f, 255), true);
					isAttacking = true;
					
					this->DisableInput(MyController);
					//myMovementComponent->StopAllMovement();
					float randomDmg = FMath::RandRange(0.0f, 100.0f);
					ApplyAttackEffect(randomDmg, (ACharacter*)attackHit[i].GetActor());
					//AAIPlayer *ai = (AAIPlayer*)attackHit.GetActor();
					//ai->TakeMentalDamage(randomDmg);

					ARPoolCurrent--;
					//isAttacking = false;

					bCanAttackAgain = false;
					attackCD = 0.0f;
				}
			}
		}
		//isAttacking = false;
	}
}

void AHumanPlayer::StartDefendAction()
{
	isDefending = true;
	canMove = false;
	PlayerMesh->SetVisibility(false);
	defenseiveObjectMesh->SetVisibility(true);
	if (!freezeCameraOnDefendDoOnce)
	{
		cameraRotation.X = 0.0f;
		freezeCameraOnDefendDoOnce = true;
	}
}

void AHumanPlayer::StopDefendAction()
{
	PlayerMesh->SetVisibility(true);
	defenseiveObjectMesh->SetVisibility(false);
	isDefending = false;
	canMove = true;

	freezeCameraOnDefendDoOnce = false;
	
}

void AHumanPlayer::PickUpAction()
{
	if (pickUpItem != emptyItem)
	{
		UE_LOG(LogTemp, Warning, TEXT("Attempting to pickup item now"));
		bool test = playerInventory->PickUpItem(pickUpItem);

		if (test)
		{
			//DestroyItemOnPickupEvent();
			pickupComponent->GetOwner()->Destroy();
			pickupComponent = nullptr;
			bIsPickingUpObject = true;
			UE_LOG(LogTemp, Warning, TEXT("ItemPickedUp"));
		}
		else if (pickUpItem.itemType == "Interactable")
		{
			((UInteractOnlyItem*)pickupComponent)->InteractWithPlayer();
			pickupComponent = nullptr;
			pickUpItem = emptyItem;
		}
		else
		{
			UE_LOG(LogTemp, Warning, TEXT("Can't pick up item or interact"));
		}
	}
}

void AHumanPlayer::UseSlotOne()
{
	TArray<FItem> inventoryArray = playerInventory->GetInventoryArray();

	if (inventoryArray[1].itemName != "NULL" && inventoryArray[0].itemName != "EMPTY")
	{
		UE_LOG(LogTemp, Warning, TEXT("using: %s"), *inventoryArray[0].itemName)
		ARPoolCurrent = ARPoolMax;
		playerInventory->UseSlotOne();
		//bIsPickingUpObject = true;
	}
}

void AHumanPlayer::UseSlotTwo()
{
	TArray<FItem> inventoryArray = playerInventory->GetInventoryArray();

	if ((inventoryArray[1].itemName != "NULL" && inventoryArray[1].itemName != "EMPTY") && bCanuseKeyCard)
	{
		//ARPoolCurrent = ARPoolMax;
		if (doorToUnlock != nullptr)
		{
			if (doorToUnlock->DoorLockType == inventoryArray[1].itemName)
			{
				doorToUnlock->bDoorLocked = false;
				doorToUnlock = nullptr;
				playerInventory->UseSlotTwo();
				//bIsPickingUpObject = true;
			}
			else
			{
				UE_LOG(LogTemp, Warning, TEXT("KEYS DO NOT MATCH | %s != %s"), *doorToUnlock->DoorLockType, *inventoryArray[1].itemName);
			}
		}
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("Itemnames == EMPTY or NULL and bCanUseKeyCard: %s"), bCanuseKeyCard ? TEXT("TRUE") : TEXT("FALSE"));
	}
}

void AHumanPlayer::UseSlotThree()
{
	TArray<FItem> inventoryArray = playerInventory->GetInventoryArray();

	if (inventoryArray[2].itemName != "NULL" && inventoryArray[2].itemName != "EMPTY")
	{
		//ARPoolCurrent = ARPoolMax;
		playerInventory->UseSlotThree();
		//bIsPickingUpObject = true;
	}
}

void AHumanPlayer::UseSlotFour()
{
	TArray<FItem> inventoryArray = playerInventory->GetInventoryArray();

	if (inventoryArray[3].itemName != "NULL" && inventoryArray[3].itemName != "EMPTY")
	{
		//ARPoolCurrent = ARPoolMax;
		playerInventory->UseSlotFour();
		//bIsPickingUpObject = true;
	}
}

void AHumanPlayer::AddARToPool(float inputAmount)
{
	ARPoolCurrent = FMath::Clamp(ARPoolCurrent += inputAmount, 0.0f, (float)ARPoolMax);
}

float AHumanPlayer::GetARPoolRatio()
{
	return (ARPoolCurrent / ARPoolMax);
}

void AHumanPlayer::AttackEnd()
{

}

float AHumanPlayer::GetDRPoolRatio()
{
	return (DRPoolCurrent / DRPoolMax);
}

void AHumanPlayer::PlayerCaught()
{
	isCaught = true;

	
}

bool AHumanPlayer::GetPlayerCaught()
{
	return isCaught;
}

void AHumanPlayer::StopCameraInput()
{
	cameraRotation.X = 0.0f;
	cameraRotation.Y = 0.0f;
}

void AHumanPlayer::SetPickUpItem(FItem item, UInventoryItemComponent* pickUpComp)
{
	pickUpItem = item;
	pickupComponent = pickUpComp;
}

void AHumanPlayer::ClearPickUpItem()
{
	pickUpItem = emptyItem;
}

void AHumanPlayer::SetPickUpObjectStatus(bool newStatus)
{
	bIsPickingUpObject = false;
}

void AHumanPlayer::AttackDone()
{
	isAttacking = false;
	//myMovementComponent->EnableAllMovement();
}

bool AHumanPlayer::DoesPlayerHoldAKeyCard()
{
	return playerInventory->CheckIfSlotIsOccupied(1);
}

void AHumanPlayer::SetDoorToUnlockBool(UDoorLockComponent* doorComp)
{
	doorToUnlock = doorComp;
}

void AHumanPlayer::ReportNoise(USoundBase* SoundToPlay, float Volume)
{
	//If we have a valid sound to play, play the sound and
	//report it to our game
	//MakeNoise(Volume, this, GetActorLocation());
	if (SoundToPlay)
	{
		//Play the actual sound
		UGameplayStatics::PlaySoundAtLocation(GetWorld(), SoundToPlay, GetActorLocation(), Volume);

		//Report that we've played a sound with a certain volume in a specific location
		MakeNoise(0.8f, this, GetActorLocation());


	}
}

void AHumanPlayer::SavePlayerData()
{
	FPlayerPersistentData tempData;

	tempData.currentARPool = ARPoolCurrent;
	tempData.inventorySlotOne = playerInventory->GetInventoryArray()[0];
	tempData.inventorySlotTwo = playerInventory->GetInventoryArray()[1];
	tempData.inventorySlotThree = playerInventory->GetInventoryArray()[2];
	tempData.inventorySlotFour = playerInventory->GetInventoryArray()[3];

	GetWorld()->GetGameInstance<UHologramGameInstance>()->UpdatePersistantPlayer(tempData);
}

void AHumanPlayer::LoadPlayerData(FPlayerPersistentData tempData)
{
	ARPoolCurrent = tempData.currentARPool;
	//playerInventory->GetInventoryArray()[0] = tempData.inventorySlotOne;
	//playerInventory->GetInventoryArray()[1] = tempData.inventorySlotTwo;
	//playerInventory->GetInventoryArray()[2] = tempData.inventorySlotThree;
	//playerInventory->GetInventoryArray()[3] = tempData.inventorySlotFour;

	playerInventory->PickUpItem(tempData.inventorySlotOne);
	playerInventory->PickUpItem(tempData.inventorySlotTwo);
	playerInventory->PickUpItem(tempData.inventorySlotThree);
	playerInventory->PickUpItem(tempData.inventorySlotFour);
	UE_LOG(LogTemp, Warning, TEXT("After data load"));
	UE_LOG(LogTemp, Warning, TEXT("Data being Saved: AmmoCount: %f | InventorySlotOneName : %s | InventorySlotOneName : %s | InventorySlotOneName : %s | InventorySlotOneName : %s"), ARPoolCurrent, *playerInventory->GetInventoryArray()[0].itemName, *playerInventory->GetInventoryArray()[1].itemName, *playerInventory->GetInventoryArray()[2].itemName, *playerInventory->GetInventoryArray()[3].itemName);
}
