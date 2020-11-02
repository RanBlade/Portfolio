// Copyright 1998-2019 Epic Games, Inc. All Rights Reserved.

#include "Game.h"
#include "Modules/ModuleManager.h"

IMPLEMENT_PRIMARY_GAME_MODULE( FDefaultGameModuleImpl, Game, "Game" );


FItem::FItem()
{
	itemName = "NULL";
	itemType = "NULL";
	itemCount = -1;

	
}