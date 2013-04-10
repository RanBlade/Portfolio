using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

class GameMessageWriter : MessageWriter
{

    public void Encode_FunctionUserLogin(ref string t_username,
                                            ref string t_psswrd)
    {
        AddInt2u((ushort)Function_Codes.FUNCTION_USER_LOGIN);
        AddStr(t_username);
        AddStr(t_psswrd);

    }
    public void Encode_FunctionUserChat(ref string t_userName,
                                         ref string t_chatMsg,
                                         ref UInt64 t_id)
    {
        AddInt2u((ushort)Function_Codes.FUNCTION_USER_CHAT);
        AddStr(t_chatMsg);
        AddInt8u(t_id);
        AddStr(t_userName);
    }
    public void Encode_FunctionRequestGame()
    {
        AddInt2u((ushort)Function_Codes.FUNCTION_MAINROOM_REQUEST_GAME);
    }
    public void Encode_FunctionRequestLeaveGame()
    {
        AddInt2u((ushort)Function_Codes.FUNCTION_TANKROOM_REQUEST_LEAVE);
    }
    public void Encode_FunctionTankUpdatePOSHPR(float x, float y, float z, float h, float p, float r)
    {
        AddInt2u((ushort)Function_Codes.FUNCTION_TANKROOM_USER_MOVE_TANK);
        AddFloat(x);
        AddFloat(y);
        AddFloat(z);
        AddFloat(h);
        AddFloat(p);
        AddFloat(r);
    }
    public void Encode_FunctionTankUpdateTankRotation( short t_direction)
    {
        AddInt2u((ushort)Function_Codes.FUNCTION_TANKROOM_USER_ROTATE_TANK);
        AddInt2(t_direction);
    }
    public void Encode_FunctionTankUpdateTurretRotation()
    {

    }
};
