using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

class GameMessageReader : MessageReader
{
    private ushort m_functionCode;
    public void InitAdapter(ref BinaryMessage t_msg)
    {
        m_readAdapterMsg = t_msg;
        GetInt2u(ref m_functionCode);
    }
    public ushort GetFunctionCode() { return m_functionCode; }
    public void ResetToFuncCode() { m_readAdapterMsg.ResetRead(); }
    public void Decode_FunctionUserLoginResp(ref int t_returnCode,
                                             ref UInt64 t_id,
                                             ref string t_username)
    {
        GetInt4(ref t_returnCode);
        GetInt8u(ref t_id);
        GetStr(ref t_username);
    }
    public void Decode_FunctionUserChat(ref string t_chatMsg,
                                        ref string t_userName,
                                        ref UInt64 t_whoID)
    {
        GetStr(ref t_chatMsg);
        GetInt8u(ref t_whoID);
        GetStr(ref t_userName);
    }
    public void Decode_FunctionUserNewRoom(ref ushort t_roomType,
                                             ref string t_roomName)
    {
        GetInt2u(ref t_roomType);
        GetStr(ref t_roomName);
    }
    public void Decode_FunctionUserInRoom(ref List<User> t_list,
                                             ref ushort userCount)
    {
        ushort t_userCount = 0;
        GetInt2u(ref t_userCount);
        userCount = t_userCount;

        for (int i = 0; i < t_userCount; i++)
        {
            User t_user = new User();
            GetInt8u(ref t_user.m_id);
            Debug.Log("User: " + t_user.m_id.ToString() + " added");
            GetStr(ref t_user.m_username);
            Debug.Log("      " + t_user.m_username.ToString());
            t_list.Add(t_user);
        }

    }
    public void Decode_FunctionUserOutRoom( ref ulong t_userID )
    {
        GetInt8u(ref t_userID);
    }
    public void Decode_FunctionTankUpdatePOSHPR( ref float x , ref float y,
                                              ref float z, ref float h,
                                              ref float p, ref float r)
    {
        GetFloat(ref x);
        GetFloat(ref y);
        GetFloat(ref z);
        GetFloat(ref h);
        GetFloat(ref p);
        GetFloat(ref r);
    }
    public void Decode_FunctionTankUpdateRotation(ref short t_direction)
    {
        GetInt2(ref t_direction);
    }
};
