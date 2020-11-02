using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OMILGT.Utilities;

public class MoveForwardCommandP : Command
{
    public MoveForwardCommandP()
    {
        commandName = "MoveForward";
    }

    public override void Execute(GameObject currentUser)
    {
        base.Execute(currentUser);

        currentUser.GetComponent<PlayerAgent>().AddMovementInput(new Vector3(0.0f, 0.0f, 1.0f));
    }
}

public class MoveBackwardCommandP : Command
{
    public MoveBackwardCommandP()
    {
        commandName = "MoveBack";
    }

    public override void Execute(GameObject currentUser)
    {
        base.Execute(currentUser);

        currentUser.GetComponent<PlayerAgent>().AddMovementInput(new Vector3(0.0f, 0.0f, -1.0f));
    }
}

public class MoveRightCommandP : Command
{
    public MoveRightCommandP()
    {
        commandName = "MoveRight";
    }

    public override void Execute(GameObject currentUser)
    {
        base.Execute(currentUser);

        currentUser.GetComponent<PlayerAgent>().AddMovementInput(new Vector3(1.0f, 0.0f, 0.0f));
    }
}

public class MoveLeftCommandP : Command
{
    public MoveLeftCommandP()
    {
        commandName = "MoveLeft";
    }

    public override void Execute(GameObject currentUser)
    {
        base.Execute(currentUser);

        currentUser.GetComponent<PlayerAgent>().AddMovementInput(new Vector3(-1.0f, 0.0f, 0.0f));
    }
}

public class StopMoveCommandP : Command
{
    public StopMoveCommandP()
    {
        commandName = "Stop";
    }

    public override void Execute(GameObject currentUser)
    {
        base.Execute(currentUser);

        currentUser.GetComponent<PlayerAgent>().AddMovementInput(new Vector3(0.0f, 0.0f, 0.0f));
    }
}

public class DashCommandP : Command
{
    public DashCommandP()
    {
        commandName = "Dash";
    }

    public override void Execute(GameObject currentUser)
    {
        base.Execute(currentUser);

        currentUser.GetComponent<PlayerAgent>().ActivateDash();
    }
}

