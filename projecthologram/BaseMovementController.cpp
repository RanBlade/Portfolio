// Fill out your copyright notice in the Description page of Project Settings.


#include "BaseMovementController.h"
#include "game.h"

#include "Engine.h"
#include "GameFramework/Actor.h"


UBaseMovementComponent::UBaseMovementComponent()
{
	//SetComponentTickEnabled(true);
	baseMovementSpeed = WALKSPEED;
	runSpeed = SPRINTSPEED;
	walkSpeed = WALKSPEED;
	jumpSpeed = 1600.0f;
	maxJumpHeight = 65.0f;

	isJumping = false;
	isCurrentlyFalling = false;
	bIsAllowedToMove = true;

	verticalVelocityLastTick = -1.0f;

	UE_LOG(LogTemp, Warning, TEXT("UBaseMovementCompoent constructor called %s"), *GetName());
}

void UBaseMovementComponent::BeginPlay()
{
	baseMovementSpeed = walkSpeed;
}

void UBaseMovementComponent::TickComponent(float DeltaTime, enum ELevelTick TickType, FActorComponentTickFunction *ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
	//UE_LOG(LogTemp, Warning, TEXT("%s UpdateCalled"), *GetName());
    Velocity = FVector::ZeroVector;

	if (!PawnOwner || !UpdatedComponent || ShouldSkipUpdate(DeltaTime))
	{
		return;
	}
	if (bIsAllowedToMove)
	{
		//handle jump
		if (isJumping)
		{
			float compareJumpHeight = (UpdatedComponent->GetComponentLocation().Z - jumpStartVec.Z);
			if (compareJumpHeight < maxJumpHeight)
			{
				//UE_LOG(LogTemp, Warning, TEXT("JumpHeight: %f"), compareJumpHeight);
				FHitResult jHit;

				FVector upVec = FVector(0.0f, 0.0f, 1.0f);
				AddInputVector((upVec* jumpSpeed * DeltaTime) + (Velocity));

				FVector jumpThisFrame = ConsumeInputVector();

				SafeMoveUpdatedComponent(jumpThisFrame, UpdatedComponent->GetComponentRotation(), true, jHit);

				if (jHit.IsValidBlockingHit())
				{
					isJumping = false;
					isCurrentlyFalling = true;
					UE_LOG(LogTemp, Warning, TEXT("Hit something while jumping"));
					if (jHit.IsValidBlockingHit())
					{
						SlideAlongSurface(jumpThisFrame, 1.0f - jHit.Time, jHit.Normal, jHit);
					}
				}
			}
			else
			{
				isJumping = false;
			}
			//UE_LOG(LogTemp, Warning, TEXT("isJumping: %s"), isJumping ? TEXT("True") : TEXT("False"));
		}

		movementInput = movementInput * baseMovementSpeed;
		FVector clampedMovementInput = movementInput.GetClampedToSize(0.0f, baseMovementSpeed);
		AddInputVector(clampedMovementInput * DeltaTime);
		
		FVector DesiredMovementThisFrame = ConsumeInputVector();

		Velocity += clampedMovementInput;

		currentSpeed = Velocity.Size();

		if (!DesiredMovementThisFrame.IsNearlyZero())
		{
			FHitResult hit;
			SafeMoveUpdatedComponent(DesiredMovementThisFrame, UpdatedComponent->GetComponentRotation(), true, hit);

			if (hit.IsValidBlockingHit())
			{
				SlideAlongSurface(DesiredMovementThisFrame, 1.0f - hit.Time, hit.Normal, hit);
			}
		}
	}
	//handle gravity
	//isCurrentlyFalling = CheckIfonGround();
	if (isCurrentlyFalling)
	{
		FHitResult ghit;
		FVector gravity = FVector(0.0f, 0.0f, -980.0f);
		AddInputVector(gravity * DeltaTime);

		FVector GravityThisFrame = ConsumeInputVector();

		Velocity += gravity;

		SafeMoveUpdatedComponent(GravityThisFrame, UpdatedComponent->GetComponentRotation(), true, ghit);
	}
	isCurrentlyFalling = CheckIfonGround();

	//UE_LOG(LogTemp, Warning, TEXT("IsCurrentlyFalling: %s"), isCurrentlyFalling ? TEXT("TRUE") : TEXT("FALSE"));
	//UE_LOG(LogTemp, Warning, TEXT("IsJumping: %s"), isJumping ? TEXT("TRUE") : TEXT("FALSE"));
	UpdateComponentVelocity();
	
	movementInput = FVector::ZeroVector;
}

void UBaseMovementComponent::StartJump()
{
	if (!isJumping && !isCurrentlyFalling)
	{
		jumpStartVec = UpdatedComponent->GetComponentLocation();
		isJumping = true;		

		//UE_LOG(LogTemp, Warning, TEXT("UpdateComponentAllowing the jump"));
	}
}

bool UBaseMovementComponent::CheckIfonGround()
{
	FHitResult *groundHit = new FHitResult();
	
	FVector offset = FVector(0.0f, 0.0f, -95.0f);
	FVector startTrace = UpdatedComponent->GetComponentLocation() + offset;
	FVector directionVec = -UpdatedComponent->GetUpVector();
	
	FVector endTrace = ((directionVec * 12.0f) + startTrace);
	FCollisionQueryParams *TraceParams = new FCollisionQueryParams();
	
	//DrawDebugLine(GetWorld(), startTrace, endTrace, FColor(255, 0, 0), true);

	if (GetWorld()->LineTraceSingleByChannel(*groundHit, startTrace, endTrace, ECC_GameTraceChannel5, *TraceParams))
	{
		if (groundHit->GetComponent())
		{	
			//UE_LOG(LogTemp, Warning, TEXT("%s has landed on the ground %s"), *GetName(), *groundHit->GetActor()->GetName());
			return false;			
		}
	}
	//UE_LOG(LogTemp, Warning, TEXT("%s in the air apply gravity"), *GetName());
	return true;
}

void UBaseMovementComponent::AddMovementVector(FVector input)
{
	//Velocity = FVector::ZeroVector;

	movementInput += input;
	//Velocity += input * baseMovementSpeed;

	//UpdateComponentVelocity();
}

void UBaseMovementComponent::SetMovementSpeed(float inSpeed)
{
	baseMovementSpeed = inSpeed;
}

void UBaseMovementComponent::SetRunSpeedActive()
{
	baseMovementSpeed = runSpeed;
}

void UBaseMovementComponent::SetWalkSpeedActive()
{
	baseMovementSpeed = walkSpeed;
}

void UBaseMovementComponent::StopAllMovement()
{
	bIsAllowedToMove = false;
}

void UBaseMovementComponent::EnableAllMovement()
{
	bIsAllowedToMove = true;
}
